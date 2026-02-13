using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using KidproblemService.Helpers;
using KidproblemService.Interfaces;
using KidproblemService.Models;
using System.Text.RegularExpressions;

namespace KidproblemService.Services
{
    public class SummaryService : ISummaryService
    {
        private readonly IDynamoDBContext _context;
        private readonly ICacheService _cacheService;

        public SummaryService(IDynamoDBContext context, ICacheService cacheService)
        {
            _context = context;
            _cacheService = cacheService;
        }

        public async Task<ExamSummary?> GetExamSummaryAsync(string category, string answerBy)
        {
            ExamSummary? entity = _cacheService.Get<ExamSummary>(category, answerBy);
            if (entity != null)
            {
                return entity;
            }

            QueryOperationConfig queryConfig = new QueryOperationConfig
            {
                KeyExpression = new()
                {
                    ExpressionAttributeValues = new() {
                        { ":category", category },
                        { ":answer_by", answerBy },
                    },
                    ExpressionStatement = "category = :category AND answer_by = :answer_by"
                }
            };

            var entities = await _context.FromQueryAsync<ExamSummary>(queryConfig).GetRemainingAsync();
            entity = entities.FirstOrDefault();
            if (entity != null)
            {
                _cacheService.Set(entity);
            }
            return entity;
        }

        public async Task<ProblemSummary?> GetProblemSummaryAsync(string problemTitle, string answerBy)
        {
            ProblemSummary? entity = _cacheService.Get<ProblemSummary>(problemTitle, answerBy);
            if (entity != null)
            {
                return entity;
            }

            QueryOperationConfig queryConfig = new QueryOperationConfig
            {
                KeyExpression = new()
                {
                    ExpressionAttributeValues = new() {
                        { ":problem_title", problemTitle },
                        { ":answer_by", answerBy },
                    },
                    ExpressionStatement = "problem_title = :problem_title AND answer_by = :answer_by"
                }
            };

            var entities = await _context.FromQueryAsync<ProblemSummary>(queryConfig).GetRemainingAsync();
            entity = entities.FirstOrDefault();
            if (entity != null)
            {
                // Many problem summaries have wrong TrueCorrectCount data. Returen the correct value.
                if (entity.TrueCorrectRate == 0 && entity.CorrectCount > 0 && entity.TotalCount > 0)
                {
                    entity.TrueCorrectRate = (double)(entity.CorrectCount ?? 0 - entity.GuessCorrectCount ?? 0) / entity.TotalCount;
                }
                _cacheService.Set(entity);
            }
            return entity;
        }

        public async Task<Tuple<List<ExamSummary>, string?>> QueryExamSummariesAsync(string familyId, string answerBy, bool usePagination = false, int? pageSize = 25, string? paginationToken = null)
        {
            List<ExamSummary> result = new();

            if (string.IsNullOrWhiteSpace(answerBy))
            {
                return new Tuple<List<ExamSummary>, string?>(result, null);
            }

            var table = _context.GetTargetTable<ExamSummary>();
            QueryOperationConfig queryConfig = new QueryOperationConfig
            {
                IndexName = "family_id-answer_by-index",
                Select = SelectValues.AllProjectedAttributes,
                KeyExpression = new()
                {
                    ExpressionAttributeValues = new() {
                        { ":answer_by", answerBy },
                        { ":family_id", familyId },
                    },
                    ExpressionStatement = "answer_by = :answer_by AND family_id = :family_id"
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
            result.AddRange(_context.FromDocuments<ExamSummary>(entities));
            return new Tuple<List<ExamSummary>, string?>(result, paginationToken);
        }

        public async Task<Tuple<List<ProblemSummary>, string?>> QueryProblemSummariesAsync(string category, string? answerBy, string? problemTitle, string? trueCorrectRateRange, bool usePagination = false, int? pageSize = 25, string? paginationToken = null)
        {
            List<ProblemSummary> result = new();

            var table = _context.GetTargetTable<ProblemSummary>();
            QueryOperationConfig queryConfig = new QueryOperationConfig
            {
                IndexName = "category-answer_by-index",
                Select = SelectValues.AllProjectedAttributes
            };

            queryConfig.Filter.AddCondition("category", QueryOperator.Equal, category);
            if (!string.IsNullOrWhiteSpace(answerBy))
            {
                queryConfig.Filter.AddCondition("answer_by", QueryOperator.Equal, answerBy);
            }

            double minRate = 0;
            double maxRate = 1;
            if (!string.IsNullOrEmpty(trueCorrectRateRange))
            {
                string pattern = @"(0\.\d+)";
                RegexOptions options = RegexOptions.Singleline;
                int i = 0;
                foreach (Match m in Regex.Matches(trueCorrectRateRange, pattern, options))
                {
                    if (i == 0)
                    {
                        double.TryParse(m.Value, out minRate);
                        i++;
                    }
                    else if (i == 1)
                    {
                        double.TryParse(m.Value, out maxRate);
                        break;
                    }
                }
                queryConfig.Filter.AddCondition("true_correct_rate", QueryOperator.Between, minRate, maxRate);
            }
            if (!string.IsNullOrEmpty(problemTitle))
            {
                queryConfig.Filter.AddCondition("problem_title", ScanOperator.Contains, problemTitle.ToUpper());
            }

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

            // Many problem summaries have wrong TrueCorrectCount data. Returen the correct value.
            var summaries = _context.FromDocuments<ProblemSummary>(entities).ToArray();
            foreach (var s in summaries)
            {
                if (s.TrueCorrectRate == 0 && s.CorrectCount > 0 && s.TotalCount > 0)
                {
                    s.TrueCorrectRate = (double)(s.CorrectCount ?? 0 - s.GuessCorrectCount ?? 0) / s.TotalCount;
                }
            }
            result.AddRange(summaries.Where(s => (s.TrueCorrectRate ?? 0) >= minRate && (s.TrueCorrectRate ?? 0) <= maxRate));
            return new Tuple<List<ProblemSummary>, string?>(result, paginationToken);
        }

        public async Task UpdateExamSummaryAsync(ExamRun examRun)
        {
            string answerBy = examRun.AnswerBy!;
            string familyId = examRun.FamilyId!;

            foreach (var detail in examRun.ExamRunDetails!)
            {
                string problemCategory = ProblemHelper.GetCategoryFromProblemTitle(detail.ProblemTitle!);
                var summary = await GetExamSummaryAsync(problemCategory, answerBy);
                if (summary == null)
                {
                    summary = new ExamSummary()
                    {
                        ProblemCategory = problemCategory,
                        AnswerBy = answerBy,
                        FamilyId = familyId,
                        TotalCount = 0,
                        CorrectCount = 0,
                        GuessCount = 0,
                        GuessCorrectCount = 0,
                        TotalDuration = 0
                    };
                }

                summary.TotalCount++;
                if (detail.IsCorrect.GetValueOrDefault()) summary.CorrectCount++;
                if (detail.IsGuess.GetValueOrDefault()) summary.GuessCount++;
                if (detail.IsGuess.GetValueOrDefault() && detail.IsCorrect.GetValueOrDefault()) summary.GuessCorrectCount++;
                summary.TotalDuration += detail.Duration;

                await _context.SaveAsync(summary);
            }
        }

        public async Task UpdateProblemSummaryAsync(ExamRun examRun)
        {
            string answerBy = examRun.AnswerBy!;
            string familyId = examRun.FamilyId!;

            foreach (var detail in examRun.ExamRunDetails!)
            {
                var summary = await GetProblemSummaryAsync(detail.ProblemTitle!, answerBy);
                string problemCategory = ProblemHelper.GetCategoryFromProblemTitle(detail.ProblemTitle!);
                if (summary == null)
                {
                    summary = new ProblemSummary()
                    {
                        ProblemTitle = detail.ProblemTitle,
                        ProblemCategory = problemCategory,
                        AnswerBy = answerBy,
                        FamilyId = familyId,
                        TotalCount = 0,
                        CorrectCount = 0,
                        GuessCount = 0,
                        GuessCorrectCount = 0,
                        TrueCorrectRate = 0,
                        TotalDuration = 0
                    };
                }

                summary.TotalCount++;
                if (detail.IsCorrect.GetValueOrDefault()) summary.CorrectCount++;
                if (detail.IsGuess.GetValueOrDefault()) summary.GuessCount++;
                if (detail.IsGuess.GetValueOrDefault() && detail.IsCorrect.GetValueOrDefault()) summary.GuessCorrectCount++;
                if (summary.TotalCount > 0)
                {
                    summary.TrueCorrectRate = (double)(summary.CorrectCount ?? 0 - summary.GuessCorrectCount ?? 0) / summary.TotalCount;
                }
                summary.TotalDuration += detail.Duration;

                await _context.SaveAsync(summary);
            }
        }
    }
}
