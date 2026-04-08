using KpUiTestxUnit.Pages;

namespace KpUiTestxUnit.Tests
{
    public class MenuBarTest : TestBase
    {

        public MenuBarTest(SetupFixture fixture) : base(fixture, true)
        {

        }

        [Fact]
        public void User_Nagivage_To_List_All_Exam_Runs()
        {
            RunTest(() =>
            {
                _driver.Navigate().GoToUrl($"{Constants.HOME_URL}");
                var page = new MenuBarComponentPage(_driver);
                page.ClickExamRunsBrowseAll();
                Assert.True(_wait.Until(d => d.Url.Contains("/examruns/all")));
                Assert.Equal("List All Exam Runs", page.GetPageTitle());
            });
        }

        [Fact]
        public void User_Nagivage_To_List_Latest_Exam_Runs()
        {
            RunTest(() =>
            {
                _driver.Navigate().GoToUrl($"{Constants.HOME_URL}");
                var page = new MenuBarComponentPage(_driver);
                page.ClickExamRunsBrowseLatest();
                Assert.True(_wait.Until(d => d.Url.Contains("/examruns")));
                Assert.Equal("List Latest Exam Runs", page.GetPageTitle());
            });
        }

        [Fact]
        public void User_Nagivage_To_List_Exam_Summary()
        {
            RunTest(() =>
            {
                _driver.Navigate().GoToUrl($"{Constants.HOME_URL}");
                var page = new MenuBarComponentPage(_driver);
                page.ClickExamRunsExamSummary();
                Assert.True(_wait.Until(d => d.Url.Contains("/summary/exam")));
                Assert.Equal("List Exam Summaries", page.GetPageTitle());
            });
        }

        [Fact]
        public void User_Nagivage_To_List_Problem_Summary()
        {
            RunTest(() =>
            {
                _driver.Navigate().GoToUrl($"{Constants.HOME_URL}");
                var page = new MenuBarComponentPage(_driver);
                page.ClickExamRunsProblemSummary();
                Assert.True(_wait.Until(d => d.Url.Contains("/summary/problem")));
                Assert.Equal("List Problem Summaries", page.GetPageTitle());
            });
        }

        [Fact]
        public void User_Nagivage_To_List_All_Assignments()
        {
            RunTest(() =>
            {
                _driver.Navigate().GoToUrl($"{Constants.HOME_URL}");
                var page = new MenuBarComponentPage(_driver);
                var destPage = new AssignmentListPage(_driver);
                page.ClickBrowseAllAssignments();
                Assert.True(_wait.Until(d => d.Url.Contains("/assignments/all")));
                Assert.Equal("List All Assignments", page.GetPageTitle());
            });
        }

        [Fact]
        public void User_Nagivage_To_List_Latest_Assignments()
        {
            RunTest(() =>
            {
                _driver.Navigate().GoToUrl($"{Constants.HOME_URL}");
                var page = new MenuBarComponentPage(_driver);
                var destPage = new AssignmentListPage(_driver);
                page.ClickBrowseLatestAssignments();
                Assert.True(_wait.Until(d => d.Url.Contains("/assignments")));
                Assert.Equal("List Latest Assignments", page.GetPageTitle());
            });
        }

        [Fact]
        public void User_Nagivage_To_List_Active_Exam_Definition()
        {
            RunTest(() =>
            {
                _driver.Navigate().GoToUrl($"{Constants.HOME_URL}");
                var page = new MenuBarComponentPage(_driver);
                page.ClickBrowseActiveExamDefinition();
                Assert.True(_wait.Until(d => d.Url.Contains("/examdefs")));
                Assert.Equal("List Active Exam Definitions", page.GetPageTitle());
            });
        }

        [Fact]
        public void User_Nagivage_To_List_All_Exam_Definition()
        {
            RunTest(() =>
            {
                _driver.Navigate().GoToUrl($"{Constants.HOME_URL}");
                var page = new MenuBarComponentPage(_driver);
                page.ClickBrowseAllExamDefinition();
                Assert.True(_wait.Until(d => d.Url.Contains("/examdefs/all")));
                Assert.Equal("List All Exam Definitions", page.GetPageTitle());
            });
        }

        [Fact]
        public void User_Nagivage_To_Create_Exam_Definition()
        {
            RunTest(() =>
            {
                _driver.Navigate().GoToUrl($"{Constants.HOME_URL}");
                var page = new MenuBarComponentPage(_driver);
                page.ClickCreateExamDefinition();
                Assert.True(_wait.Until(d => d.Url.Contains("/examdef/create")));
                Assert.Equal("Edit Exam Definition", page.GetPageTitle());
            });
        }

        [Fact]
        public void User_Nagivage_To_Browse_Problems()
        {
            RunTest(() =>
            {
                _driver.Navigate().GoToUrl($"{Constants.HOME_URL}");
                var page = new MenuBarComponentPage(_driver);
                page.ClickBrowseProblems();
                Assert.True(_wait.Until(d => d.Url.Contains("/problems/r")));
                Assert.Equal("List Problems", page.GetPageTitle());
            });
        }

        [Fact]
        public void User_Nagivage_To_Create_Problem()
        {
            RunTest(() =>
            {
                _driver.Navigate().GoToUrl($"{Constants.HOME_URL}");
                var page = new MenuBarComponentPage(_driver);
                page.ClickCreateProblem();
                Assert.True(_wait.Until(d => d.Url.Contains("/problem/create")));
                Assert.Equal("Problem Editor", page.GetPageTitle());
            });
        }

        [Fact]
        public void User_Nagivage_To_Browse_Staging_Problems()
        {
            RunTest(() =>
            {
                _driver.Navigate().GoToUrl($"{Constants.HOME_URL}");
                var page = new MenuBarComponentPage(_driver);
                page.ClickBrowseStagingProblems();
                Assert.True(_wait.Until(d => d.Url.Contains("/problems/s")));
                Assert.Equal("List Problems (Staging)", page.GetPageTitle());
            });
        }

        [Fact]
        public void User_Nagivage_To_Crawl_Problem()
        {
            RunTest(() =>
            {
                _driver.Navigate().GoToUrl($"{Constants.HOME_URL}");
                var page = new MenuBarComponentPage(_driver);
                page.ClickCrawlProblems();
                Assert.True(_wait.Until(d => d.Url.Contains("/problem/scrap")));
                Assert.Equal("Get Problems from AoP", page.GetPageTitle());
            });
        }

        [Fact]
        public void User_Nagivage_To_Upload_Answers()
        {
            RunTest(() =>
            {
                _driver.Navigate().GoToUrl($"{Constants.HOME_URL}");
                var page = new MenuBarComponentPage(_driver);
                page.ClickUploadAnswers();
                Assert.True(_wait.Until(d => d.Url.Contains("/problem/answers")));
                Assert.Equal("Upload Answers", page.GetPageTitle());
            });
        }

        [Fact]
        public void User_Nagivage_To_Bulk_Update()
        {
            RunTest(() =>
            {
                _driver.Navigate().GoToUrl($"{Constants.HOME_URL}");
                var page = new MenuBarComponentPage(_driver);
                page.ClickBulkUpdate();
                Assert.True(_wait.Until(d => d.Url.Contains("/problem/bulkupdate")));
                Assert.Equal("Problem Bulk Editor - Move problems out of Staging area", page.GetPageTitle());
            });
        }

        [Fact]
        public void User_Nagivage_To_Bulk_Create()
        {
            RunTest(() =>
            {
                _driver.Navigate().GoToUrl($"{Constants.HOME_URL}");
                var page = new MenuBarComponentPage(_driver);
                page.ClickBulkCreate();
                Assert.True(_wait.Until(d => d.Url.Contains("/problem/bulkcreate")));
                Assert.Equal("Get Problems from AoPS Community area", page.GetPageTitle());
            });
        }

    }
}
