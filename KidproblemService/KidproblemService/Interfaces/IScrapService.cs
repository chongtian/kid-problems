using KidproblemService.Models;

namespace KidproblemService.Interfaces
{
    public interface IScrapService
    {
        Problem[] ProcessProblems(Problem[] problems);
        Task<List<Problem>> GetProblemsAsync(ScrapDefinition definition);
        Tuple<string, string> ProcessProblemImage(string problemText, string problemTitle);
    }
}
