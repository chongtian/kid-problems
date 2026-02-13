using KidproblemService.Interfaces;
using KidproblemService.Models;

namespace KidproblemService.Controllers
{
    public class UserInfoMiddleware
    {
        private readonly RequestDelegate _next;

        public UserInfoMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IAuthenticateService authService)
        {
            var userInfo = context.RequestServices.GetRequiredService<TokenUser>();

            var currentUser = context.User;
            var tokenUser = authService.GetTokenUserFromHttpContext(currentUser);

            userInfo.Access = tokenUser.Access;
            userInfo.Username = tokenUser.Username;
            userInfo.FullName = tokenUser.FullName;
            userInfo.FamilyId = tokenUser.FamilyId;

            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }
    }
}
