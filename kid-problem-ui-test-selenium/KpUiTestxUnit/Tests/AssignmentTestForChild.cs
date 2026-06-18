using KpUiTestxUnit.Pages;

namespace KpUiTestxUnit.Tests
{
    public class AssignmentTestForChild : TestBase
    {
        public AssignmentTestForChild(SetupFixture fixture) : base(fixture, false)
        { }

        [Theory]
        [InlineData("8/1/2023", "8/15/2023", "9", "August 14, 2023, 9:03:58 AM GMT-5")]
        public void Child_User_Query_In_Assignments(string startTime, string endTime, string expectedCount, string firstRecord)
        {
            RunTest(() =>
            {
                var page = new AssignmentListPage(_driver);
                page.GoTo()
                .SetQueryDateRange(startTime, endTime)
                .ClickSearchButton();

                Assert.Equal(expectedCount, page.GetCountOfQueryResults());
                var records = page.GetAssignmentsFromQueryResults();
                Assert.True(records.Length > 0);
                Assert.Contains(firstRecord, records[0].CreateTime);
            });

        }

    }
}