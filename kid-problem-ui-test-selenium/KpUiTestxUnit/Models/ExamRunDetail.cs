namespace KpUiTestxUnit.Models;

public class ExamRunDetail
{
    public string? ProblemTitle { get; set; }
    public string? UserAnswer { get; set; }
    public string? Correct { get; set; }
    public string? Guess { get; set; }
    public string? Duration { get; set; }

    public string? Id {get;set;}
    public bool? IsGuess {get;set;}
    public bool? IsCorrect {get;set;}

    public override string ToString()
    {
        return $"{ProblemTitle}{UserAnswer}{Correct}{Guess}{Duration}";
    }
}
