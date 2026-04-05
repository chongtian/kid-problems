namespace KpUiTestxUnit.Models;

public class ExamRunDetails
{
    public required string ProblemTitle { get; set; }
    public string? UserAnswer { get; set; }
    public string? Correct { get; set; }
    public string? Guess { get; set; }
    public string? Duration { get; set; }

    public override string ToString()
    {
        return $"{ProblemTitle}{UserAnswer}{Correct}{Guess}{Duration}";
    }
}
