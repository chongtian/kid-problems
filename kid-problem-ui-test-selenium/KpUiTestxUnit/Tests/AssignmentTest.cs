using KpUiTestxUnit.Data;
using KpUiTestxUnit.Models;
using KpUiTestxUnit.Pages;
using NuGet.Frameworks;

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
            RunTest(() =>
            {
                var page = new AssignmentListPage(_driver);
                page.GoTo();

                page.SetQueryDateRange(startTime, endTime);
                page.ClickSearchButton();
                Assert.Equal(expectedCount, page.GetCountOfQueryResults());
                var records = page.GetAssignmentsFromQueryResults();
                Assert.True(records.Length > 0);
                Assert.Contains(firstRecord, records[0].CreateTime);
            });

        }

        [Theory]
        [InlineData("8/1/2023", "8/15/2023", 9, "e4a21255-272c-4c8c-a657-e667db863b98", "AMC10 Review 140 520")]
        [InlineData("10/1/2023", "10/25/2023", 5, "a358a6e5-a429-4ab5-b73d-a3a4a043f114", "AMC10-2022B Part 4 575")]
        public void User_Navigate_To_Detail_From_List_Assignments(string startTime, string endTime, int selectIndex, string uid, string examTitle)
        {
            RunTest(() =>
            {
                var page = new AssignmentListPage(_driver);
                page.GoTo();

                page.SetQueryDateRange(startTime, endTime);
                page.ClickSearchButton();
                page.ClickAssignmentTitleInQueryResults(selectIndex);
                Assert.True(_wait.Until(d => d.Url.Contains($"{Constants.BASE_URL}/assignment/view/{uid}")));
                var destPage = new AssignmentViewPage(_driver);
                Assert.Equal(examTitle, destPage.GetExamTitle());
            });
        }

        [Theory]
        [MemberData(nameof(DataForAssignment))]
        public void User_View_Assignment(Assignment testData)
        {
            RunTest(() =>
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
            });
        }

        [Fact]
        public void User_Navigates_Pages_Of_Assignments()
        {
            RunTest(() =>
            {
                var page = new AssignmentListPage(_driver);
                page.GoTo();

                page.SetQueryDateRange("4/1/2023", "4/30/2023");
                page.ClickSearchButton();
                Assert.Equal("25", page.GetCountOfQueryResults());

                page.Paginator.ClickNextPageButton();
                var records = page.GetAssignmentsFromQueryResults();
                Assert.Equal(10, records.Length);
                Assert.Contains("April 6, 2023, 8:16:01 AM GMT-5", records[9].CreateTime);

                page.Paginator.ClickLastPageButton();
                records = page.GetAssignmentsFromQueryResults();
                Assert.Equal(5, records.Length);
                Assert.Contains("AMC10 Review 068 434", records[0].ExamTitle);

                page.Paginator.ClickPreviousPageButton();
                records = page.GetAssignmentsFromQueryResults();
                Assert.Equal(10, records.Length);
                Assert.Contains("April 6, 2023, 8:16:01 AM GMT-5", records[9].CreateTime);

                page.Paginator.ClickFirstPageButton();
                records = page.GetAssignmentsFromQueryResults();
                Assert.Equal(10, records.Length);
                Assert.Contains("AMC10 Review 088 454", records[0].ExamTitle);
            });
        }

        [Fact]
        public void User_Load_More_Assignments()
        {
            RunTest(() =>
            {
                var page = new AssignmentListPage(_driver);
                page.GoTo();

                page.SetQueryDateRange("4/1/2023", "4/30/2023");
                page.ClickSearchButton();
                Assert.Equal("25", page.GetCountOfQueryResults());
                Assert.True(page.IsLoadMoreButtonShown());

                page.ClickMoreButton();
                Assert.Equal("28", page.GetCountOfQueryResults());
                Assert.True(page.IsLoadMoreButtonHidden());

                page.Paginator.ClickLastPageButton();
                var records = page.GetAssignmentsFromQueryResults();
                Assert.Equal(8, records.Length);
                Assert.Contains("AMC10 Review 089 455", records[7].ExamTitle);
                
            });
        }

    }
}
