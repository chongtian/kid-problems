using KpUiTestxUnit.Pages;

namespace KpUiTestxUnit.Tests
{
    public class DashboardTestForAdmin : TestBase
    {

        public DashboardTestForAdmin(SetupFixture fixture) : base(fixture, true)
        {

        }

        [Fact]
        public void Dashboard_Should_Display_List_Exam_Summaries()
        {
            RunTest(() =>
            {
                var page = new DashboardPage(_driver);
                var items = page.GetDashboardItemTitles();
                Assert.NotEmpty(items);
                Assert.Contains("List Exam Summaries", items[0]);
            });
        }

    }
}
