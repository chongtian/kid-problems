using KidproblemService.Interfaces;
using KidproblemService.Models;

namespace KidproblemService.Helpers
{
    public static class PopulateUserFullNameExtensionMethod
    {
        public static T PopulateUserFullName<T>(this T entity, IAuthenticateService authService) where T : BaseModelWithUserName
        {
            if (!string.IsNullOrEmpty(entity.GetAnswerBy()) && string.IsNullOrEmpty(entity.AnswerByFullname))
            {
                entity.AnswerByFullname = authService.GetUserNameAsync(entity.GetAnswerBy() ?? string.Empty).Result;
            }
            return entity;
        }

        public static IEnumerable<T> PopulateUserFullName<T>(this IEnumerable<T> entities, IAuthenticateService authService) where T : BaseModelWithUserName
        {
            foreach(var entity in entities)
            {
                if (!string.IsNullOrEmpty(entity.GetAnswerBy()) && string.IsNullOrEmpty(entity.AnswerByFullname))
                {
                    entity.AnswerByFullname = authService.GetUserNameAsync(entity.GetAnswerBy() ?? string.Empty).Result;
                }
            }
            return entities;
        }
    }
}
