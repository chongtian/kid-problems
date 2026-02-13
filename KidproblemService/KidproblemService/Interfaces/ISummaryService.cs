using KidproblemService.Models;

namespace KidproblemService.Interfaces
{
    public interface ISummaryService
    {
        Task<ProblemSummary?> GetProblemSummaryAsync(string problemTitle, string answerBy);
        Task<Tuple<List<ProblemSummary>, string?>> QueryProblemSummariesAsync(string category, string? answerBy, string? problemTitle, string? trueCorrectRateRange, bool usePagination = false, int? pageSize = 25, string? paginationToken = null);
        Task<ExamSummary?> GetExamSummaryAsync(string category, string answerBy);
        Task<Tuple<List<ExamSummary>, string?>> QueryExamSummariesAsync(string familyId, string answerBy, bool usePagination = false, int? pageSize = 25, string? paginationToken = null);
        Task UpdateProblemSummaryAsync(ExamRun examRun);
        Task UpdateExamSummaryAsync(ExamRun examRun);
    }
}
