using Amazon.CognitoIdentityProvider.Model;
using Amazon.CognitoIdentityProvider;
using KidproblemService.Interfaces;
using KidproblemService.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace KidproblemService.Services
{
    public class TokenAuthenticationService : IAuthenticateService
    {
        private readonly TokenUser _currentUserInfo;
        private readonly string _cognitoUserPoolId;
        private readonly IAmazonCognitoIdentityProvider _cognitoService;
        private readonly IMemoryCache _memoryCache;

        private const string UserFullNameCacheKey = "CognitoUserFullName:";
        private const string ChildrenCacheKey = "CognitoChildren:";
        private const string UnkownUserName = "Unknown User";

        public TokenAuthenticationService(
            TokenUser currentUserInfo,
            IAmazonCognitoIdentityProvider cognitoService,
            IOptions<AwsConfiguration> awsConfiguration,
            IMemoryCache memoryCache
            )
        {
            _currentUserInfo = currentUserInfo;
            _cognitoUserPoolId = awsConfiguration.Value.UserPoolId!;
            _cognitoService = cognitoService;
            _memoryCache = memoryCache;
        }

        public async Task<bool> VerifyIfChildInFamilyAsync(string childUserName, string familyId)
        {
            var allChildren = await GetAllChildrenAsync(familyId);
            return allChildren.Contains(childUserName);
        }

        public async Task<List<string>> GetAllChildrenAsync(string familyId)
        {
            string cacheEntry = ChildrenCacheKey + familyId;

            if (!_memoryCache.TryGetValue(cacheEntry, out List<string>? usernames))
            {
                ListUsersInGroupRequest userRequest = new ListUsersInGroupRequest
                {
                    GroupName = "ChildUserGroup",
                    UserPoolId = _cognitoUserPoolId,
                };

                try
                {
                    usernames = new();
                    var response = await _cognitoService.ListUsersInGroupAsync(userRequest);
                    response.Users.ForEach(u => usernames.Add(u.Username));

                    userRequest = new ListUsersInGroupRequest
                    {
                        GroupName = familyId,
                        UserPoolId = _cognitoUserPoolId,
                    };
                    response = await _cognitoService.ListUsersInGroupAsync(userRequest);
                    var familyUsers = new List<string>();
                    response.Users.ForEach(u => familyUsers.Add(u.Username));

                    usernames = usernames.Intersect(familyUsers).ToList();

                    if (usernames.Count > 0)
                    {
                        var cacheEntryOptions = new MemoryCacheEntryOptions()
                            .SetSlidingExpiration(TimeSpan.FromDays(120));
                        _memoryCache.Set(cacheEntry, usernames, cacheEntryOptions);
                    }
                }
                catch (Exception)
                {
                    // do nothing
                }
            }

            return usernames!;
        }

        public async Task<string> GetUserNameAsync(string userName)
        {
            string cacheEntry = UserFullNameCacheKey + userName;

            if (!_memoryCache.TryGetValue(cacheEntry, out string? userFullName))
            {
                AdminGetUserRequest userRequest = new AdminGetUserRequest
                {
                    Username = userName,
                    UserPoolId = _cognitoUserPoolId,
                };

                try
                {
                    var response = await _cognitoService.AdminGetUserAsync(userRequest);
                    userFullName = response.UserAttributes.FirstOrDefault(a => a.Name == "name")?.Value ?? UnkownUserName;
                }
                catch (UserNotFoundException)
                {
                    userFullName = UnkownUserName;
                }

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(120));

                _memoryCache.Set(cacheEntry, userFullName, cacheEntryOptions);
            }

            return userFullName!;
        }

        public TokenUser GetCurrentUserInfo()
        {
            return _currentUserInfo;
        }

        public TokenUser GetTokenUserFromHttpContext(ClaimsPrincipal user)
        {
            string username = user.Claims.FirstOrDefault(c => c.Type == "username")?.Value!;
            string? fullname = user.Claims.FirstOrDefault(c => c.Type == "name")?.Value ?? UnkownUserName;

            var groups = user.Claims.Where(c => c.Type == "cognito:groups");

            // parse familyId
            var familyId = groups.FirstOrDefault(g => g.Value.Contains("FamilyGroup"))?.Value ?? "MainFamilyGroup";

            // parse Access
            var userGroup = groups.FirstOrDefault(g => g.Value.Contains("UserGroup"))?.Value ?? "ChildUserGroup";
            int access = 0;
            switch (userGroup)
            {
                case "AdminUserGroup":
                    access = 7;
                    break;
                case "ParentUserGroup":
                    access = 3;
                    break;
                case "ChildUserGroup":
                    access = 1;
                    break;
                default:
                    access = 0;
                    break;
            }
            return new TokenUser()
            {
                Access = access,
                FamilyId = familyId,
                Username = username,
                FullName = fullname
            };
        }


    }
}
