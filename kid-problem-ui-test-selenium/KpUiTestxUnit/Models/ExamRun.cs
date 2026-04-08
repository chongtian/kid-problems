namespace KpUiTestxUnit.Models;

public class ExamRun
{
    public string? UID { get; set; }
    public string? ExamCategory { get; set; }
    public string? ExamTitle { get; set; }
    public string? AnsweredBy { get; set; }
    public string? StartTime { get; set; }
    public string? CompleteTime { get; set; }
    public string? TotalDuration { get; set; }
    public int? TotalCount { get; set; }
    public int? CorrectCount { get; set; }
    public int? GuessCount { get; set; }
    public int? GuessCorrectCount { get; set; }

    public ExamRunDetail[]? ExamDetails { get; set; }

}

