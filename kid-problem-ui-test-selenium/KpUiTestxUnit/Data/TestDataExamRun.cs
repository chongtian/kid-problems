using KpUiTestxUnit.Models;

namespace KpUiTestxUnit.Data;

public static class TestDataExamRun
{
    public static readonly ExamRun Test1 = new()
    {
        UID = "1ff0b9d5-f065-4a3f-8b26-230cb649ca10",
        ExamCategory = "AMC10",
        ExamTitle = "AMC8 Review 048 413",
        AnsweredBy = "Yinkai Gao",
        StartTime = "March 12, 2023, 3:57:41 PM GMT-5",
        CompleteTime = "March 12, 2023, 4:30:52 PM GMT-5",
        TotalDuration = "1,991",
        TotalCount = 8,
        CorrectCount = 5,
        GuessCount = 7,
        GuessCorrectCount = 4,
        ExamDetails = [
            new ExamRunDetails() {ProblemTitle = "AMC10-2021A-020", UserAnswer = "C", Correct = "No", Guess = "Yes", Duration = "101"},
            new ExamRunDetails() {ProblemTitle = "AMC10-2021A-021", UserAnswer = "C", Correct = "No", Guess = "Yes", Duration = "170"},
            new ExamRunDetails() {ProblemTitle = "AMC10-2021A-022", UserAnswer = "B", Correct = "No", Guess = "No", Duration = "313"},
            new ExamRunDetails() {ProblemTitle = "AMC10-2021FA-015", UserAnswer = "C", Correct = "No", Guess = "Yes", Duration = "287"},
            new ExamRunDetails() {ProblemTitle = "AMC10-2021FA-019", UserAnswer = "D", Correct = "No", Guess = "Yes", Duration = "401"},
            new ExamRunDetails() {ProblemTitle = "AMC10-2021FA-022", UserAnswer = "D", Correct = "No", Guess = "Yes", Duration = "348"},
            new ExamRunDetails() {ProblemTitle = "AMC10-2021FA-020", UserAnswer = "B", Correct = "No", Guess = "Yes", Duration = "14"},
            new ExamRunDetails() {ProblemTitle = "AMC10-2021B-020", UserAnswer = "D", Correct = "No", Guess = "Yes", Duration = "354"},
        ]
    };
}
