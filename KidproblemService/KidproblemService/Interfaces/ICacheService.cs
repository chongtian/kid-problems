using KidproblemService.Models;

namespace KidproblemService.Interfaces
{
    public interface ICacheService
    {
        T? Get<T>(string key1, string? key2 = null) where T : BaseModel;
        void Unset<T>(string key1, string? key2 = null) where T : BaseModel;
        void Set<T>(T entity) where T : BaseModel;
        void Set<T>(IEnumerable<T> entities) where T : BaseModel;
    }
}
