using Amazon.CognitoIdentityProvider.Model;
using Amazon.DynamoDBv2.DataModel;
using KidproblemService.Interfaces;
using KidproblemService.Models;
using Microsoft.Extensions.Caching.Memory;

namespace KidproblemService.Services
{
    public class CodeService: ICodeService
    {
        private readonly IDynamoDBContext _context;
        private readonly IMemoryCache _memoryCache;
        private const string CodeCacheKey = "KIDPROBLEM_CODE_";

        public CodeService(IDynamoDBContext context, IMemoryCache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;
        }

        /// <summary>
        /// Return Codes by code name
        /// </summary>
        /// <param name="codeName"></param>
        /// <returns></returns>
        public async Task<List<CodeDetail>?> GetCodeDetailsAsync(string codeName)
        {
            string cacheEntry = CodeCacheKey + codeName.ToUpper();

            if (!_memoryCache.TryGetValue(cacheEntry, out List<CodeDetail>? codes))
            {
                var entity = await _context.LoadAsync<InfoCentralCode>(codeName.ToUpper());
                if (entity?.CodeDetails?.Count > 0)
                {
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                           .SetSlidingExpiration(TimeSpan.FromDays(120));
                    _memoryCache.Set(cacheEntry, entity?.CodeDetails, cacheEntryOptions);
                }
                codes = entity?.CodeDetails;
            }

            return codes;
        }

        /// <summary>
        /// Save Codes.
        /// This method accepts any code_name. The caller shall verify if the code_name is correct.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<InfoCentralCode> UpdateCodeDetailsAsync(InfoCentralCode code)
        {
            await _context.SaveAsync(code);
            string cacheEntry = CodeCacheKey + code.CodeName!.ToUpper();
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                           .SetSlidingExpiration(TimeSpan.FromDays(120));
            _memoryCache.Set(cacheEntry, code.CodeDetails, cacheEntryOptions);
            return code;
        }

        /// <summary>
        /// Check if the code name is a valid one.
        /// </summary>
        /// <param name="codeName"></param>
        /// <returns></returns>
        public bool IsValidCodeName(string codeName)
        {
            var validCodeNames = new string[] { "KIDPROBLEM_CATEGORIES" };
            return Array.Exists(validCodeNames, a => a == codeName.ToUpper());
        }
    }
}
