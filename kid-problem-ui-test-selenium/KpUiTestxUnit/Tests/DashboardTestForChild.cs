using KpUiTestxUnit.Pages;

namespace KpUiTestxUnit.Tests
{
    public class DashboardTestForChild : TestBase
    {

        public DashboardTestForChild(SetupFixture fixture) : base(fixture, false)
        {

        }

        [Fact]
        public void Dashboard_Should_Display_Exam_Statistics()
        {
            RunTest(() =>
            {
                var page = new DashboardPage(_driver);
                var items = page.GetChildExamStatistics();
                Assert.NotEmpty(items);
                Assert.True(items.Length > 1);
            });
        }

    }
}
