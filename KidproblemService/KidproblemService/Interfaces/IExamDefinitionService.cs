using KidproblemService.Models;

namespace KidproblemService.Interfaces
{
    public interface IExamDefinitionService
    {
        Task<ExamDefinition?> GetExamDefinitionAsync(string category, string title);
        Task<Tuple<List<ExamDefinition>, string?>> QueryExamDefinitionsAsync(string category, bool? active, string keyword, bool usePagination = false, int? pageSize = 25, string? paginationToken = null);
        Task<ExamDefinition> CreateAsync(ExamDefinition entity);
        Task<ExamDefinition> UpdateAsync(ExamDefinition entity);
        Task<ExamDefinition> DeleteAsync(ExamDefinition entity);
    }
}
