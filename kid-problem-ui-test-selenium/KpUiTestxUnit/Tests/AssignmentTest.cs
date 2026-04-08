using KpUiTestxUnit.Data;
using KpUiTestxUnit.Models;
using KpUiTestxUnit.Pages;

namespace KpUiTestxUnit.Tests
{
    public class AssignmentTest : TestBase
    {
        public static IEnumerable<object?[]> DataForAssignment()
        {
            yield return new object?[] { TestDataAssignment.Test1 };
        }

        public AssignmentTest(SetupFixture fixture) : base(fixture, true)
        {

        }

        [Theory]
        [InlineData("8/1/2023", "8/15/2023", "12", "August 14, 2023, 9:03:58 AM GMT-5")]
        [InlineData("10/1/2023", "10/25/2023", "14", "October 23, 2023, 2:36:59 PM GMT-5")]
        public void User_Query_In_Assignments(string startTime, string endTime, string expectedCount, string firstRecord)
        {
            var page = new AssignmentListPage(_driver);
            page.GoTo();

            page.SetQueryDateRange(startTime, endTime);
            page.ClickSearchButton();
            Assert.Equal(expectedCount, page.GetCountOfQueryResults());
            var records = page.GetAssignmentsFromQueryResults();
            Assert.True(records.Length > 0);
            Assert.Contains(firstRecord, records[0].CreateTime);
        }

        [Theory]
        [InlineData("8/1/2023", "8/15/2023", 9, "e4a21255-272c-4c8c-a657-e667db863b98", "AMC10 Review 140 520")]
        [InlineData("10/1/2023", "10/25/2023", 5, "a358a6e5-a429-4ab5-b73d-a3a4a043f114", "AMC10-2022B Part 4 575")]
        public void User_Navigate_To_Detail_From_List_Assignments(string startTime, string endTime, int selectIndex, string uid, string examTitle)
        {
            var page = new AssignmentListPage(_driver);
            page.GoTo();

            page.SetQueryDateRange(startTime, endTime);
            page.ClickSearchButton();
            page.ClickAssignmentTitleInQueryResults(selectIndex);
            Assert.True(_wait.Until(d => d.Url.Contains($"{Constants.BASE_URL}/assignment/view/{uid}")));
            var destPage = new AssignmentViewPage(_driver);
            Assert.Equal(examTitle, destPage.GetExamTitle());
        }

        [Theory]
        [MemberData(nameof(DataForAssignment))]
        public void User_View_Assignment(Assignment testData)
        {
            var page = new AssignmentViewPage(_driver);
            page.GoTo(testData.UID!);

            Assert.Equal(testData.ExamCategory, page.GetExamCategory());
            Assert.Equal(testData.ExamTitle, page.GetExamTitle());
            Assert.Equal(testData.CreateTime, page.GetCreateTime());
            Assert.Equal(testData.Memo, page.GetMemo());
            Assert.Equal(testData.Completed, page.GetCompleted());

            var details = page.GetExamRuns();
            for (int i = 0; i < testData.ExamRuns!.Length; i++)
            {
                Assert.Equal(testData.ExamRuns[i], details[i].Item1);
            }
        }
    }
}
