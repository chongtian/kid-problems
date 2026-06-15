using KidproblemService.Models;

namespace KidproblemService.Interfaces
{
    public interface IAssignmentService
    {
        Task<Assignment?> GetAssignmentAsync(string id);
        Task<Tuple<List<Assignment>, string?>> QueryAssignmentsAsync(string familyId, string? childId, DateTime start, DateTime end, bool usePagination = false, int? pageSize = 25, string? paginationToken = null);
        Task<Assignment> CreateAssignmentFromDefinitionAsync(ExamDefinition entity, string familyId);
        Task<Assignment> UpdateAsync(Assignment entity, Assignment existing);
        Task<Assignment> DeleteAsync(Assignment entity);
    }
}
