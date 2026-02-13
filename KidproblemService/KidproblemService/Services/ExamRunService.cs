using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using KidproblemService.Interfaces;
using KidproblemService.Models;

namespace KidproblemService.Services
{
    public class ExamRunService : IExamRunService
    {
        private readonly IDynamoDBContext _context;
        private readonly IProblemService _problemService;
        private readonly IExamDefinitionService _examDefinitionService;
        private readonly IAssignmentService _assignmentService;
        private readonly ICacheService _cacheService;
        private readonly ISummaryService _summaryService;
        private readonly ICodeService _codeService;

        private readonly string ConfigurationKeyName = "KIDPROBLEM_CONFIG";
        private readonly string SingleUserModeKeyName = "SINGLE_USER";

        public ExamRunService(IDynamoDBContext context, IProblemService problemService, ICacheService cacheService, IExamDefinitionService examDefinitionService, IAssignmentService assignmentService, ISummaryService summaryService, ICodeService codeService)
        {
            _context = context;
            _problemService = problemService;
            _cacheService = cacheService;
            _examDefinitionService = examDefinitionService;
            _assignmentService = assignmentService;
            _summaryService = summaryService;
            _codeService = codeService;
        }

        /// <summary>
        /// Set the exam run as completed and do some summarization
        /// </summary>
        /// <param name="entity">an already existing exam</param>
        /// <returns></returns>
        public async Task<ExamRun> CompleteExamAsync(ExamRun entity)
        {
            if (entity.CompleteTime.HasValue)
            {
                // this exam run is already completed
                entity.IsSuccessful = false;
                entity.ReturnResult = "Exam Run has already been completed.";
                return entity;
            }

            entity.CompleteTime = DateTime.UtcNow;
            entity.TotalDuration = (entity.CompleteTime.Value - entity.StartTime!.Value!).TotalSeconds;
            // reset the counts
            entity.CorrectCount = entity.GuessCount = entity.GuessCorrectCount = 0;
            entity.TotalCount = entity.ExamRunDetails!.Count;

            // summarize exam run details
            List<ExamRunDetail> details = new();
            foreach (var runDetail in entity.ExamRunDetails!)
            {
                string runDetailId = runDetail.Id!;
                var actualDetail = await GetExamRunDetailAsync(runDetailId) ?? runDetail;
                details.Add(actualDetail);

                // get the correct answer
                string problemTitle = actualDetail.ProblemTitle!;
                var problem = await _problemService.GetProblemAsync(problemTitle);
                if (problem == null)
                {
                    entity.ReturnResult += $"Cannot find Problem {problemTitle}. ";
                    entity.IsSuccessful = false;
                    continue;
                }
                else
                {
                    if ((problem.ProblemAnswer ?? string.Empty).Trim() == (actualDetail.UserAnswer ?? string.Empty).Trim())
                    {
                        entity.CorrectCount++;
                        if (actualDetail.IsGuess.GetValueOrDefault())
                        {
                            entity.GuessCorrectCount++;
                        }
                        actualDetail.IsCorrect = true;
                    }
                    else
                    {
                        actualDetail.IsCorrect = false;
                    }

                    if (actualDetail.IsGuess.GetValueOrDefault())
                    {
                        entity.GuessCount++;
                    }
                }
            }
            entity.ExamRunDetails = details;
            await _context.SaveAsync(entity);
            _cacheService.Set(entity);

            // summarize exam run
            await _summaryService.UpdateExamSummaryAsync(entity);
            // summarize problem
            await _summaryService.UpdateProblemSummaryAsync(entity);

            // To Do: delete exam run details
            //foreach (var runDetail in entity.ExamRunDetails!)
            //{
            //    string runDetailId = runDetail.Id!;
            //    var actualDetail = await GetExamRunDetailAsync(runDetailId) ?? runDetail;
            //    await _context.DeleteAsync(actualDetail);
            //}

            // implement single user mode
            try
            {
                var codes = await _codeService.GetCodeDetailsAsync(ConfigurationKeyName);
                var code = codes?.FirstOrDefault(c => c.Code == SingleUserModeKeyName);
                var singleUser = code != null && code.Active.GetValueOrDefault(false);
                if (singleUser)
                {
                    var examRun = await GetExamRunWithoutDetailsAsync(entity.Id!);
                    if (examRun != null)
                    {
                        var asn = await _assignmentService.GetAssignmentAsync(examRun.AssignmentId!);
                        if (asn != null)
                        {
                            asn.IsComplete = true;
                            asn = await _assignmentService.UpdateAsync(asn, asn);
                            var examDef = await _examDefinitionService.GetExamDefinitionAsync(asn.ExamCategory!, asn.ExamTitle!);
                            if (examDef != null && examDef.ExamType == ExamDefinitionService.ExamTypeHome)
                            {
                                examDef.Active = false;
                                await _examDefinitionService.UpdateAsync(examDef);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return entity;
        }

        private async Task<ExamRun?> GetExamRunWithoutDetailsAsync(string id)
        {
            ExamRun? entity;

            QueryOperationConfig queryConfig = new QueryOperationConfig
            {
                KeyExpression = new()
                {
                    ExpressionAttributeValues = new() {
                        { ":id", id },
                    },
                    ExpressionStatement = "id = :id"
                }
            };

            var entities = await _context.FromQueryAsync<ExamRun>(queryConfig).GetRemainingAsync();
            entity = entities.FirstOrDefault();
            return entity;
        }

        

        public async Task<ExamRun> CreateExamRunFromAssignmentAsync(Assignment assignment, string answerBy)
        {
            var examDefinition = await _examDefinitionService.GetExamDefinitionAsync(assignment.ExamCategory!, assignment.ExamTitle!);
            if (examDefinition == null)
            {
                return new ExamRun()
                {
                    ReturnResult = $"Invalid exam definition is associated with Assignment {assignment.Id}.",
                    IsSuccessful = false
                };
            }
            var examRunDetails = new List<ExamRunDetail>();
            foreach (var examDetail in examDefinition.ExamDetails!)
            {
                var examRunDetail = new ExamRunDetail()
                {
                    Id = Guid.NewGuid().ToString(),
                    ProblemTitle = examDetail.ProblemTitle,
                    UserAnswer = string.Empty,
                    IsGuess = false,
                    IsCorrect = false,
                    Duration = 0
                };
                examRunDetails.Add(examRunDetail);
                await _context.SaveAsync(examRunDetail);
            }

            ExamRun examRun = new()
            {
                Id = Guid.NewGuid().ToString(),
                AnswerBy = answerBy,
                CreateTime = DateTime.UtcNow,
                StartTime = DateTime.UtcNow,
                AssignmentId = assignment.Id,
                ExamCategory = examDefinition.ExamCategory,
                ExamTitle = examDefinition.ExamTitle,
                FamilyId = assignment.FamilyId,
                TotalCount = examRunDetails.Count,
                CorrectCount = 0,
                GuessCount = 0,
                GuessCorrectCount = 0,
                TotalDuration = 0,
                ExamRunDetails = examRunDetails
            };
            await _context.SaveAsync(examRun);

            if (assignment.ExamRunIds == null) assignment.ExamRunIds = new();
            assignment.ExamRunIds!.Add(examRun.Id);
            await _context.SaveAsync(assignment);
            return examRun;
        }

        /// <summary>
        /// This method deletes ExamRunDetail records associated with the given ExamRun.
        /// It also removes the Run Id from the associated Assignment.ExamRunIds.
        /// In the end, it deletes the given ExamRun record.
        /// </summary>
        /// <param name="entity">An ExamRun entity with all attributes projected</param>
        /// <returns></returns>
        public async Task<ExamRun> DeleteAsync(ExamRun entity)
        {
            if (entity.CompleteTime.HasValue || entity.ExamRunDetails!.Any(d => d.Duration.GetValueOrDefault() > 0))
            {
                entity.ReturnResult = "Cannot delete an exam run that has been completed or executed.";
                entity.IsSuccessful = false;
                return entity;
            }

            foreach (var runDetail in entity.ExamRunDetails!)
            {
                await _context.DeleteAsync(runDetail);
            }

            string assignmentId = entity.AssignmentId!;
            var assignment = await _assignmentService.GetAssignmentAsync(assignmentId ?? string.Empty);
            if (assignment != null && assignment.ExamRunIds!.Contains(entity.Id!))
            {
                assignment.ExamRunIds.Remove(entity.Id!);
                await _context.SaveAsync(assignment);
            }

            await _context.DeleteAsync(entity);
            return entity;
        }

        public async Task<ExamRun?> GetExamRunAsync(string id)
        {
            ExamRun? entity = _cacheService.Get<ExamRun>(id);
            if(entity != null)
            {
                return entity;
            }

            QueryOperationConfig queryConfig = new QueryOperationConfig
            {
                KeyExpression = new()
                {
                    ExpressionAttributeValues = new() {
                        { ":id", id },
                    },
                    ExpressionStatement = "id = :id"
                }
            };

            var entities = await _context.FromQueryAsync<ExamRun>(queryConfig).GetRemainingAsync();
            entity = entities.FirstOrDefault();
            if (entity != null && entity.ExamRunDetails != null)
            {
                entity.ExamRunDetails.OrderBy(d => d.ProblemTitle);
                if (!entity.CompleteTime.HasValue)
                {
                    for (int i = 0; i < entity.ExamRunDetails.Count; i++)
                    {
                        var detail = await GetExamRunDetailAsync(entity.ExamRunDetails[i].Id!);
                        if (detail != null)
                        {
                            entity.ExamRunDetails[i] = detail;
                        }
                    }
                }
            }
            return entity;
        }

        public async Task<ExamRunDetail?> GetExamRunDetailAsync(string id)
        {
            QueryOperationConfig queryConfig = new QueryOperationConfig
            {
                KeyExpression = new()
                {
                    ExpressionAttributeValues = new() {
                        { ":id", id },
                    },
                    ExpressionStatement = "id = :id"
                }
            };

            var entities = await _context.FromQueryAsync<ExamRunDetail>(queryConfig).GetRemainingAsync();
            var entity = entities.FirstOrDefault();
            return entity;
        }

        public async Task<Tuple<List<ExamRun>, string?>> QueryExamRunsAsync(string answerBy, DateTime start, DateTime end, bool usePagination = false, int? pageSize = 25, string? paginationToken = null)
        {
            List<ExamRun> result = new();

            if (string.IsNullOrWhiteSpace(answerBy))
            {
                return new Tuple<List<ExamRun>, string?>(result, null);
            }

            var table = _context.GetTargetTable<ExamRun>();
            QueryOperationConfig queryConfig = new QueryOperationConfig
            {
                IndexName = "answer_by-create_time-index",
                Select = SelectValues.AllProjectedAttributes,
                KeyExpression = new()
                {
                    ExpressionAttributeValues = new() {
                        { ":answer_by", answerBy },
                        { ":start", start },
                        { ":end", end },
                    },
                    ExpressionStatement = "answer_by = :answer_by AND create_time BETWEEN :start AND :end"
                }
            };

            if (usePagination)
            {
                queryConfig.Limit = pageSize.GetValueOrDefault();
                queryConfig.PaginationToken = paginationToken;
            }

            var query = table.Query(queryConfig);
            List<Document> entities;
            if (usePagination)
            {
                entities = await query.GetNextSetAsync();
                paginationToken = query.PaginationToken;
            }
            else
            {
                entities = await query.GetRemainingAsync();
            }
            result.AddRange(_context.FromDocuments<ExamRun>(entities));
            // populate all fields
            for (int i = 0; i < result.Count; i++)
            {
                var examRun = await GetExamRunAsync(result[i].Id!);
                result[i] = examRun!;
            }
            return new Tuple<List<ExamRun>, string?>(result, paginationToken);
        }

        public async Task<Tuple<List<ExamRun>, string?>> QueryExamRunsByFamilyIdAsync(string familyId, DateTime start, DateTime end, bool usePagination = false, int? pageSize = 25, string? paginationToken = null)
        {
            List<ExamRun> result = new();

            if (string.IsNullOrWhiteSpace(familyId))
            {
                return new Tuple<List<ExamRun>, string?>(result, null);
            }

            var table = _context.GetTargetTable<ExamRun>();
            QueryOperationConfig queryConfig = new QueryOperationConfig
            {
                IndexName = "family_id-create_time-index",
                Select = SelectValues.AllProjectedAttributes,
                KeyExpression = new()
                {
                    ExpressionAttributeValues = new() {
                        { ":family_id", familyId },
                        { ":start", start },
                        { ":end", end },
                    },
                    ExpressionStatement = "family_id = :family_id AND create_time BETWEEN :start AND :end"
                }
            };

            if (usePagination)
            {
                queryConfig.Limit = pageSize.GetValueOrDefault();
                queryConfig.PaginationToken = paginationToken;
            }

            var query = table.Query(queryConfig);
            List<Document> entities;
            if (usePagination)
            {
                entities = await query.GetNextSetAsync();
                paginationToken = query.PaginationToken;
            }
            else
            {
                entities = await query.GetRemainingAsync();
            }
            result.AddRange(_context.FromDocuments<ExamRun>(entities));
            // populate all fields
            for (int i = 0; i < result.Count; i++)
            {
                var examRun = await GetExamRunAsync(result[i].Id!);
                result[i] = examRun!;
            }
            return new Tuple<List<ExamRun>, string?>(result, paginationToken);
        }

        public async Task<ExamRunDetail> UpdateExamRunDetailAsync(ExamRunDetail entity)
        {
            var existing = await GetExamRunDetailAsync(entity.Id ?? string.Empty);
            if (existing != null)
            {
                existing.UserAnswer = entity.UserAnswer;
                existing.IsGuess = entity.IsGuess;
                existing.Duration += entity.Duration;
                await _context.SaveAsync(existing);
                return existing;
            }
            else
            {
                return entity;
            }
        }
    }
}
