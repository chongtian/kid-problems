using KidproblemService.Models;

namespace KidproblemService.Interfaces
{
    public interface IProblemService
    {
        Task<Problem?> GetProblemAsync(string problemTitle);
        Task<Tuple<List<Problem>, string?>> QueryProblemsAsync(string keyword, bool staging, bool usePagination = false, int? pageSize = 25, string? paginationToken = null);
        Task<Problem> CreateAsync(Problem entity);
        Task<Problem> UpdateAsync(Problem entity, Problem existing);
        Task<Problem> DeleteAsync(Problem entity);
        Task<List<Problem>> ScrapAsync(ScrapDefinition definition);
        Task<Problem[]> ScrapAsync(Problem[] problems);
    }
}
