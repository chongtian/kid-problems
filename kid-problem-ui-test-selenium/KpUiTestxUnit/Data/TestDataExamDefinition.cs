using KpUiTestxUnit.Models;

namespace KpUiTestxUnit.Data;

public static class TestDataExamDefinition
{
    public static readonly ExamDefinition CreateExamDefinition1 = new()
    {
        ExamCategory = "AMC10",
        ExamTitle = $"Test Exam Def {DateTime.Now.Ticks}",
        ActiveStatusValue = true,
        ActiveStatusText = "Yes",
        ExamYear = "2020",
        ExamType = "Practice",
        Memo = "UI Test",
        ProblemTitles =
            [
            "AMC10-2020A-001",
            "AMC10-2020A-002",
            "AMC10-2020A-003",
            "AMC10-2020A-004",
            "AMC10-2020A-005"
            ],
        ProblemTitleSelectIndexes = [0, 1, 2, 3, 4],
        CountOfExpectedProblems = 50
    };

    public static readonly ExamDefinition CreateExamDefinition2 = new()
    {
        ExamCategory = "AMC10",
        ExamTitle = $"AMC10 TestExam {DateTime.Now.Ticks}",
        ActiveStatusValue = true,
        ActiveStatusText = "Yes",
        ExamYear = "2020B",
        ExamType = "Practice",
        Memo = "UI Test",
        ProblemTitles =
        [
            "AMC10-2020B-001",
            "AMC10-2020B-002",
            "AMC10-2020B-003",
            "AMC10-2020B-004",
            "AMC10-2020B-005"
        ],
        ProblemTitleSelectIndexes = [0, 1, 2, 3, 4],
        CountOfExpectedProblems = 25
    };

    public static readonly ExamDefinition ViewExamDefinition1 = new()
    {
        ExamCategory = "AMC10",
        ExamTitle = "TestData20260210001",
        ActiveStatusValue = true,
        ActiveStatusText = "Yes",
        ExamYear = "2020",
        ExamType = "Practice",
        Memo = "UI TEST",
        ProblemTitles =
        [
            "AMC10-2020A-001",
            "AMC10-2020A-002",
            "AMC10-2020A-003",
            "AMC10-2020A-004",
            "AMC10-2020A-005"
        ],
    };  

    public static readonly ExamDefinition ViewExamDefinition2 = new()
    {
        ExamCategory = "AMC10",
        ExamTitle = "AMC10 P14 16B 236",
        ActiveStatusValue = false,
        ActiveStatusText = "No",
        ExamYear = "2013A",
        ExamType = "Practice",
        Memo = "",
        ProblemTitles =
        [
            "AMC10-2013A-014",
            "AMC10-2013B-014",
            "AMC10-2014A-014",
            "AMC10-2014B-014",
            "AMC10-2015A-014",
            "AMC10-2015B-014",
            "AMC10-2016A-014",
            "AMC10-2016B-014"
        ],
    };      
}
