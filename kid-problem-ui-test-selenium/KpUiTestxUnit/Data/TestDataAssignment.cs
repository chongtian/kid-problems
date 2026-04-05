using KpUiTestxUnit.Models;

namespace KpUiTestxUnit.Data;

public static class TestDataAssignment
{
    public static readonly Assignment Test1 = new()
    {
        UID = "5284d7ce-64fa-49e5-b73a-c512d62c1fec",
        ExamCategory = "AMC10",
        ExamTitle = "AMC10 Review 047 412",
        CreateTime = "March 10, 2023, 10:58:17 AM GMT-6",
        Memo = "",
        Completed = "Yes",
        ExamRuns = ["Run 1"]
    };
}
