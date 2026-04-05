namespace KpUiTestxUnit.Data;

public static class ProblemTestData
{
    public readonly static string RollbackUpdatePayload = @"
    {
    ""ProblemCategory"": ""TEST"",
    ""ProblemYear"": ""2026"",
    ""ProblemTitle"": ""TEST-2026-002"",
    ""ProblemNumber"": ""002"",
    ""ProblemText"": ""TEST PROBLEM TEXT LINE 1\nTEST PROBLEM TEXT LINE 2"",
    ""ProblemAnswer"": ""A"",
    ""ProblemTags"": [""PL""],
    ""IsStaging"": true,
    ""SolutionText"": ""TEST SOLUTION TEXT LINE 1<br/>\r\nTEST SOLUTION TEXT LINE 2<br/>"",
    ""AnswerOptions"": ""A,B,C,D,E"",
    ""Action"": 2
    }
    ";

}