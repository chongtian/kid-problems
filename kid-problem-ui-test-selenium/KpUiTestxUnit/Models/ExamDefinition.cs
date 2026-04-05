namespace KpUiTestxUnit.Models;

public class ExamDefinition
{
    public required string ExamCategory { get; set; }
    public required string ExamTitle { get; set; }
    public required bool ActiveStatusValue { get; set; } = true;
    public required string ActiveStatusText { get; set; } = "Yes";
    public string? ExamYear { get; set; }
    public required string ExamType { get; set; } = "Practice";
    public string? Memo { get; set; }
    public string[]? ProblemTitles { get; set; }
    public int[]? ProblemTitleSelectIndexes { get; set; }
    public int CountOfExpectedProblems { get; set; } = 0;

}
