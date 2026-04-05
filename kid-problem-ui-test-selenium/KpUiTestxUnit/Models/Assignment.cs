namespace KpUiTestxUnit.Models;

public class Assignment
{
    public string? UID { get; set; }
    public required string ExamCategory { get; set; }
    public required string ExamTitle { get; set; }
    public required string CreateTime { get; set; }
    public string? Memo { get; set; }
    public string? Completed { get; set; }

    public string[]? ExamRuns { get; set; }

}

