using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using KidproblemService.Interfaces;
using KidproblemService.Models;

namespace KidproblemService.Services
{
    public class AssignmentService : IAssignmentService
    {
        private readonly IDynamoDBContext _context;
        private readonly ICacheService _cacheService;

        public AssignmentService(IDynamoDBContext context, ICacheService cacheService)
        {
            _context = context;
            _cacheService = cacheService;
        }

        public async Task<Assignment> CreateAssignmentFromDefinitionAsync(ExamDefinition entity, string familyId)
        {
            if (entity.ExamCategory == null || entity.ExamTitle == null)
            {
                return new Assignment()
                {
                    ReturnResult = "Invalid exam definition is given.",
                    IsSuccessful = false
                };
            }

            Assignment assignment = new()
            {
                Id = Guid.NewGuid().ToString(),
                FamilyId = familyId,
                CreateTime = DateTime.UtcNow,
                ExamCategory = entity.ExamCategory,
                ExamTitle = entity.ExamTitle,
                IsComplete = false,
                Memo = "",
                ExamRunIds = new()
            };

            await _context.SaveAsync(assignment);
            return assignment;
        }

        public async Task<Assignment> DeleteAsync(Assignment entity)
        {
            if (entity.IsComplete.GetValueOrDefault() || (entity.ExamRunIds != null && entity.ExamRunIds!.Count > 0))
            {
                entity.ReturnResult = "Cannot delete an assignment that has been completed or executed.";
                entity.IsSuccessful = false;
                return entity;
            }
            await _context.DeleteAsync(entity);
            _cacheService.Unset<Assignment>(entity.Id!);
            return entity;
        }

        public async Task<Assignment?> GetAssignmentAsync(string id)
        {
            // Assignment? entity = _cacheService.Get<Assignment>(id);
            //if(entity != null)
            //{
            //    return entity;
            //}

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

            var entities = await _context.FromQueryAsync<Assignment>(queryConfig).GetRemainingAsync();
            var entity = entities.FirstOrDefault();
            if (entity != null)
            {
                _cacheService.Set(entity);
            }
            return entity;
        }

        public async Task<Tuple<List<Assignment>, string?>> QueryAssignmentsAsync(string familyId, string? childId, DateTime start, DateTime end, bool usePagination = false, int? pageSize = 25, string? paginationToken = null)
        {
            List<Assignment> result = new();

            if (string.IsNullOrWhiteSpace(familyId))
            {
                return new Tuple<List<Assignment>, string?>(result, null);
            }

            var table = _context.GetTargetTable<Assignment>();
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
            result.AddRange(_context.FromDocuments<Assignment>(entities));

            if (!string.IsNullOrWhiteSpace(childId))
            {
                // filter results. This is a temporary solution. The best way is adding child_id to index  
                result = result.Where(a => a.ChildId == childId).ToList();
            }

            // populate all fields
            for (int i = 0; i < result.Count; i++)
            {
                var assignment = await GetAssignmentAsync(result[i].Id!);
                result[i] = assignment!;
            }

            return new Tuple<List<Assignment>, string?>(result, paginationToken);
        }

        public async Task<Assignment> UpdateAsync(Assignment entity, Assignment existing)
        {
            // the associated exam definition cannot be changed
            entity.ExamCategory = existing.ExamCategory;
            entity.ExamTitle = existing.ExamTitle;
            entity.CreateTime = existing.CreateTime;
            entity.FamilyId = existing.FamilyId;
            await _context.SaveAsync(entity);
            _cacheService.Unset<Assignment>(entity.Id!);
            return entity;
        }
    }
}
