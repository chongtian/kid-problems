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
            new ExamRunDetail() {ProblemTitle = "AMC10-2021A-020", UserAnswer = "C", Correct = "No", Guess = "Yes", Duration = "101"},
            new ExamRunDetail() {ProblemTitle = "AMC10-2021A-021", UserAnswer = "C", Correct = "No", Guess = "Yes", Duration = "170"},
            new ExamRunDetail() {ProblemTitle = "AMC10-2021A-022", UserAnswer = "B", Correct = "No", Guess = "No", Duration = "313"},
            new ExamRunDetail() {ProblemTitle = "AMC10-2021FA-015", UserAnswer = "C", Correct = "No", Guess = "Yes", Duration = "287"},
            new ExamRunDetail() {ProblemTitle = "AMC10-2021FA-019", UserAnswer = "D", Correct = "No", Guess = "Yes", Duration = "401"},
            new ExamRunDetail() {ProblemTitle = "AMC10-2021FA-022", UserAnswer = "D", Correct = "No", Guess = "Yes", Duration = "348"},
            new ExamRunDetail() {ProblemTitle = "AMC10-2021FA-020", UserAnswer = "B", Correct = "No", Guess = "Yes", Duration = "14"},
            new ExamRunDetail() {ProblemTitle = "AMC10-2021B-020", UserAnswer = "D", Correct = "No", Guess = "Yes", Duration = "354"},
        ]
    };

    public static readonly ExamRunDetail[] ChildClickANswerButton = [
        new ExamRunDetail() {Id="942f1087-4a01-43db-b350-bfc5133dcbae", ProblemTitle = "AMC10-2010A-001", UserAnswer = null, IsCorrect = false, IsGuess = false, Duration = "1"},
        new ExamRunDetail() {Id="e0d699ef-c794-44a7-bb5b-c16fe30a5a92", ProblemTitle = "AMC10-2010A-002", UserAnswer = null, IsCorrect = false, IsGuess = false, Duration = "1"},
        new ExamRunDetail() {Id="085456dd-22bb-4423-bfe6-5b373dd7a593", ProblemTitle = "AMC10-2010A-003", UserAnswer = null, IsCorrect = false, IsGuess = false, Duration = "1"},
        new ExamRunDetail() {Id="6dd3028b-8f52-4dae-a21a-668d0b6cec30", ProblemTitle = "AMC10-2010A-004", UserAnswer = null, IsCorrect = false, IsGuess = false, Duration = "1"},
        new ExamRunDetail() {Id="9c5cb080-7aec-4fe8-b1b6-88da546a2f7d", ProblemTitle = "AMC10-2010A-005", UserAnswer = null, IsCorrect = false, IsGuess = false, Duration = "1"},
    ];
}
