namespace KpUiTestxUnit.Models;

public class ExamDefinition
{
    public string? ExamCategory { get; set; }
    public string? ExamTitle { get; set; }
    public bool? ActiveStatusValue { get; set; } = true;
    public string? ActiveStatusText { get; set; } = "Yes";
    public string? ExamYear { get; set; }
    public string? ExamType { get; set; } = "Practice";
    public string? Memo { get; set; }
    public string[]? ProblemTitles { get; set; }
    public int[]? ProblemTitleSelectIndexes { get; set; }
    public int? CountOfExpectedProblems { get; set; } = 0;

}
