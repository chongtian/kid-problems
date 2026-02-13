using KidproblemService.Interfaces;
using KidproblemService.Models;
using Microsoft.Extensions.Caching.Memory;

namespace KidproblemService.Services
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;

        public CacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public T? Get<T>(string key1, string? key2 = null) where T : BaseModel
        {
            string cacheEntry = GetEntryKey<T>(key1, key2);
            T? entity;
            if (_memoryCache.TryGetValue(cacheEntry, out entity))
            {
                return entity;
            } else
            {
                return null;
            }            
        }

        public void Set<T>(T entity) where T : BaseModel
        {
            string cacheEntry = GetEntryKey<T>(entity);
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromHours(2));
            _memoryCache.Set(cacheEntry, entity, cacheEntryOptions);
        }

        public void Set<T>(IEnumerable<T> entities) where T : BaseModel
        {
            foreach (var entity in entities)
            {
                Set<T>(entity);
            }
        }

        public void Unset<T>(string key1, string? key2 = null) where T : BaseModel
        {
            string cacheEntry = GetEntryKey<T>(key1, key2);
            if (_memoryCache.TryGetValue(cacheEntry, out T? _))
            {
                _memoryCache.Remove(cacheEntry);
            }
        }

        private string GetEntryKey<T>(string key1, string? key2) where T : BaseModel
        {
            string prefix;
            if (typeof(T) == typeof(Problem))
            {
                prefix = "PROB";
            }
            else if (typeof(T) == typeof(ExamDefinition))
            {
                prefix = "EXDEF";
            }
            else if (typeof(T) == typeof(Assignment))
            {
                prefix = "ASN";
            }
            else if (typeof(T) == typeof(ExamRun))
            {
                prefix = "EXRUN";
            }
            else if (typeof(T) == typeof(ExamSummary))
            {
                prefix = "EXSUM";
            }
            else if (typeof(T) == typeof(ProblemSummary))
            {
                prefix = "PRODSUM";
            }
            else
            {
                throw new NotImplementedException();
            }

            return $"{prefix}{key1}{key2}";
        }

        private string GetEntryKey<T>(T entity) where T : BaseModel
        {
            string key1;
            string? key2 = null;
            string prefix;
            if (typeof(T) == typeof(Problem))
            {
                prefix = "PROB";
                key1 = (entity as Problem)!.ProblemTitle!;
            }
            else if (typeof(T) == typeof(ExamDefinition))
            {
                prefix = "EXDEF";
                key1 = (entity as ExamDefinition)!.ExamCategory!;
                key2 = (entity as ExamDefinition)!.ExamTitle!;
            }
            else if (typeof(T) == typeof(Assignment))
            {
                prefix = "ASN";
                key1 = (entity as Assignment)!.Id!;
            }
            else if (typeof(T) == typeof(ExamRun))
            {
                prefix = "EXRUN";
                key1 = (entity as ExamRun)!.Id!;
            }
            else if (typeof(T) == typeof(ExamSummary))
            {
                prefix = "EXSUM";
                key1 = (entity as ExamSummary)!.ProblemCategory!;
                key2 = (entity as ExamSummary)!.AnswerBy!;
            }
            else if (typeof(T) == typeof(ProblemSummary))
            {
                prefix = "PRODSUM";
                key1 = (entity as ProblemSummary)!.ProblemTitle!;
                key2 = (entity as ProblemSummary)!.AnswerBy!;
            }
            else
            {
                throw new NotImplementedException();
            }

            return $"{prefix}{key1}{key2}";
        }
       
    }
}
