using KidproblemService.Models;
using System.Security.Claims;

namespace KidproblemService.Interfaces
{
    public interface IAuthenticateService
    {
        TokenUser GetCurrentUserInfo();
        Task<string> GetUserNameAsync(string userName);
        Task<List<string>> GetAllChildrenAsync(string familyId);
        TokenUser GetTokenUserFromHttpContext(ClaimsPrincipal user);
        Task<bool> VerifyIfChildInFamilyAsync(string childUserName, string familyId);
    }
}
