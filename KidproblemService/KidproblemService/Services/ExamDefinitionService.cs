using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using KidproblemService.Interfaces;
using KidproblemService.Models;

namespace KidproblemService.Services
{
    public class ExamDefinitionService : IExamDefinitionService
    {
        private readonly IDynamoDBContext _context;
        public const string ExamTypeHome = "H";
        public const string ExamTypeOfficial = "O";
        private readonly ICacheService _cacheService;

        public ExamDefinitionService(IDynamoDBContext context, ICacheService cacheService)
        {
            _context = context;
            _cacheService = cacheService;
        }

        public async Task<ExamDefinition> CreateAsync(ExamDefinition entity)
        {
            if (await Validate(entity))
            {
                await _context.SaveAsync(entity);
            }
            return entity;
        }

        public async Task<ExamDefinition> DeleteAsync(ExamDefinition entity)
        {
            await _context.DeleteAsync(entity);
            _cacheService.Unset<ExamDefinition>(entity.ExamCategory!, entity.ExamTitle);
            return entity;
        }

        public async Task<ExamDefinition?> GetExamDefinitionAsync(string category, string title)
        {
            ExamDefinition? entity = _cacheService.Get<ExamDefinition>(category, title);
            // if(entity != null)
            // {
            //     return entity;
            // }

            QueryOperationConfig queryConfig = new QueryOperationConfig
            {
                KeyExpression = new()
                {
                    ExpressionAttributeValues = new() {
                        { ":category", category.ToUpper() },
                        { ":title", title.Trim() },
                    },
                    ExpressionStatement = "category = :category AND title = :title"
                }
            };

            var entities = await _context.FromQueryAsync<ExamDefinition>(queryConfig).GetRemainingAsync();
            entity = entities.FirstOrDefault();
            if (entity != null)
            {
                _cacheService.Set(entity);
            }

            return entity;
        }

        public async Task<Tuple<List<ExamDefinition>, string?>> QueryExamDefinitionsAsync(
            string category, bool? active, string keyword, bool usePagination = false, int? pageSize = 25, string? paginationToken = null)
        {
            List<ExamDefinition> result = new();
            var table = _context.GetTargetTable<ExamDefinition>();
            QueryOperationConfig queryConfig;
            if (string.IsNullOrEmpty(keyword))
            {
                queryConfig = new QueryOperationConfig
                {
                    KeyExpression = new()
                    {
                        ExpressionAttributeValues = new() {
                        { ":category", category.ToUpper() },
                    },
                        ExpressionStatement = "category = :category "
                    }
                };
            }
            else
            {
                queryConfig = new QueryOperationConfig
                {
                    KeyExpression = new()
                    {
                        ExpressionAttributeValues = new() {
                        { ":category", category.ToUpper() },
                        { ":title", keyword.Trim() },
                    },
                        ExpressionStatement = "category = :category AND begins_with(title, :title) "
                    }
                };
            }

            if (active.HasValue && active.Value)
            {
                queryConfig.FilterExpression = new()
                {
                    ExpressionAttributeValues = new() {
                        { ":active", active.Value }
                    },
                    ExpressionStatement = "active = :active "
                };
            }

            queryConfig.ConsistentRead = true;

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

                /*
                 * This is my guess about how AWS SDK works.
                 * When there is a Query Filter, GetNextSetAsync() still retreives all records
                 * only based on the Key Condition, and then filter records using the given Query Filter.
                 * This brings an issue. For example, if there are 100 records matching with the Key Condition,
                 * but only 2 records matching with Query Filter, it is likely that one run of GetNextSetAsync() 
                 * will return only 1 record with a PaginationToken, which will effectively confused the caller.
                 * 
                 * To remediate this issue, I will check if the count of returned records is less than the given Page Size.
                 * If the count is less, then this means GetNextSetAsync() does not return all qualified records.
                 * I will call GetRemainingAsync() to get all records and set paginationToken to null. 
                 * So that the caller gets all records and know there is no need to call the API again.
                 */
                if (entities.Count < pageSize)
                {
                    var remainings = await query.GetRemainingAsync();
                    entities.AddRange(remainings);
                    paginationToken = null;
                }
                else
                {
                    paginationToken = query.PaginationToken;
                }

            }
            else
            {
                entities = await query.GetRemainingAsync();
            }
            result.AddRange(_context.FromDocuments<ExamDefinition>(entities));
            result = result.OrderBy(r => r.ExamTitle).ToList();

            return new Tuple<List<ExamDefinition>, string?>(result, paginationToken);
        }

        public async Task<ExamDefinition> UpdateAsync(ExamDefinition entity)
        {
            if (await Validate(entity))
            {
                await _context.SaveAsync(entity);
                _cacheService.Unset<ExamDefinition>(entity.ExamCategory!, entity.ExamTitle);
            }
            return entity;
        }

        private async Task<bool> Validate(ExamDefinition entity)
        {
            if (entity.ExamTitle == null || entity.ExamCategory == null || entity.ExamDetails == null || entity.ExamDetails.Count == 0)
            {
                entity.ReturnResult = "Exam Title, Exam Category or Exam Details cannot be null.";
                entity.IsSuccessful = false;
                return false;
            }

            if (entity.Action == Models.Action.Create)
            {
                var existing = await GetExamDefinitionAsync(entity.ExamCategory, entity.ExamTitle);
                if (existing != null)
                {
                    // The given exam title exists, add a suffix to the title, trying to make it unique.
                    entity.ExamTitle = $"{entity.ExamTitle} ({DateTime.UtcNow:yyMMddss})";

                    // validate it again
                    existing = await GetExamDefinitionAsync(entity.ExamCategory, entity.ExamTitle);
                    if (existing != null)
                    {
                        entity.ReturnResult = "Found duplicate Exam Definition.";
                        entity.IsSuccessful = false;
                        return false;
                    }                        
                }
            }

            entity.ExamType = entity.ExamType ?? ExamTypeHome;

            return true;
        }
    }
}
