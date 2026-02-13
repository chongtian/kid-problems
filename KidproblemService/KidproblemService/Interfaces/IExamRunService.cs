using KidproblemService.Models;

namespace KidproblemService.Interfaces
{
    public interface IExamRunService
    {
        Task<ExamRun?> GetExamRunAsync(string id);
        Task<Tuple<List<ExamRun>, string?>> QueryExamRunsAsync(string answerBy, DateTime start, DateTime end, bool usePagination = false, int? pageSize = 25, string? paginationToken = null);
        Task<Tuple<List<ExamRun>, string?>> QueryExamRunsByFamilyIdAsync(string answerBy, DateTime start, DateTime end, bool usePagination = false, int? pageSize = 25, string? paginationToken = null);
        Task<ExamRun> CreateExamRunFromAssignmentAsync(Assignment assignment, string answerBy);
        Task<ExamRun> CompleteExamAsync(ExamRun entity);
        Task<ExamRunDetail> UpdateExamRunDetailAsync(ExamRunDetail entity);
        Task<ExamRunDetail?> GetExamRunDetailAsync(string id);
        Task<ExamRun> DeleteAsync(ExamRun entity);
    }
}
