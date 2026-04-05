namespace KpUiTestxUnit.Models;

public class ProblemInfo
{
    public string? ProblemCategory { get; set; }
    public string? ProblemYear { get; set; }
    public string? ProblemTitle { get; set; }
    public string? ProblemNumber { get; set; }
    public string? ProblemText { get; set; }
    public string? ProblemAnswer { get; set; }
    public string[]? ProblemTags { get; set; }
    public bool? IsStaging { get; set; }
    public string? SolutionText { get; set; }
    public string? AnswerOptions { get; set; }
}