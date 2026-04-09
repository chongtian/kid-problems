using System.Text.Json;
using KpUiTestxUnit.Data;
using KpUiTestxUnit.Models;
using KpUiTestxUnit.Pages;
using KpUiTestxUnit.Utilties;

namespace KpUiTestxUnit.Tests
{

    public class ProblemTest : TestBase
    {
        public ProblemTest(SetupFixture fixture) : base(fixture, true)
        { }

        [Theory]
        [InlineData(false, "AMC10-2010A", 25, "AMC10-2010A-001")]
        [InlineData(true, "AMC12-2010A", 25, "AMC12-2010A-001")]
        public void User_Queries_Problem(bool isStaging, string keyword, int expectedCount, string firstRecord)
        {
            RunTest(() =>
            {
                var page = new ProblemListPage(_driver);
                page.GoTo(isStaging);
                page.EnterKeyword(keyword);
                page.ClickSearchButton();
                Assert.False(page.IsLoadMoreButtonShown());
                var results = page.GetProblemTitle();
                Assert.True(results.Length == expectedCount);
                Assert.Equal(firstRecord, results[0]);
            });

        }

        [Theory]
        [InlineData(false, "AMC10-2010", 50, "AMC10-2010B-025")]
        [InlineData(true, "AMC12-2010", 50, "AMC12-2010B-025")]
        public void User_Queries_Problem_with_Load_More(bool isStaging, string keyword, int expectedCount, string lastRecord)
        {
            RunTest(() =>
            {
                var page = new ProblemListPage(_driver);
                page.GoTo(isStaging);
                page.EnterKeyword(keyword);
                page.ClickSearchButton();
                Assert.True(page.IsLoadMoreButtonShown());
                page.ClickLoadMoreButton();
                Assert.False(page.IsLoadMoreButtonShown());
                var results = page.GetProblemTitle();
                Assert.True(results.Length == expectedCount);
                Assert.Equal(lastRecord, results[expectedCount - 1]);
            });
        }

        [Theory]
        [InlineData("AMC12-2010A", 25, "AMC12-2010A-001 Answer: C")]
        public void User_Add_Staging_Problems_In_Bulk_Editor(string keyword, int expectedCount, string firstRecord)
        {
            RunTest(() =>
            {
                var page = new ProblemBulkUpdatePage(_driver);
                page.GoTo();
                page.EnterKeyword(keyword);
                page.ClickOpenQueryButton();
                page.QueryPage.ClickSelectAllButton();
                page.QueryPage.ClickSelectButton();
                var stagingProblemTitles = page.GetStagingProblemTitles();
                var stagingProblemAnswers = page.GetStagingProblemAnswers();

                Assert.Equal(expectedCount, stagingProblemTitles.Length);
                Assert.Contains(firstRecord, $"{stagingProblemTitles[0]} {stagingProblemAnswers[0]}");
            });
        }

        [Fact]
        public void User_Generate_Problem_Answers()
        {
            RunTest(() =>
            {
                var page = new ProblemUploadAnswersPage(_driver);
                page.GoTo();
                page.EnterProblemCategory("AMC12");
                page.EnterProblemYear("2020A");
                page.EnterAnswerKeys("A\nB\nC\nD\nE");
                page.ClickGenerateButton();

                var result = page.GetGeneratedAnswerKeys();
                Assert.NotNull(result);
                var answers = result.Split("\n");
                Assert.Equal(5, answers.Length);
                Assert.Equal("AMC12-2020A-001 A", answers[0].Trim());
                Assert.Equal("AMC12-2020A-002 B", answers[1].Trim());
                Assert.Equal("AMC12-2020A-003 C", answers[2].Trim());
                Assert.Equal("AMC12-2020A-004 D", answers[3].Trim());
                Assert.Equal("AMC12-2020A-005 E", answers[4].Trim());
            });
        }

        [Fact]
        public async Task User_Creates_Problem()
        {
            await RunTestAsync(async () =>
            {
                var page = new ProblemEditPage(_driver);
                page.GoTo();

                page.EnterProblemCategory("TEST");
                page.EnterProblemYear("2026");
                page.EnterProblemNumber("001");
                page.EnterProblemAnswer("A");
                page.EnterProblemTags("Test");
                page.EnterAnswerOptions("A,B,C,D,E");
                page.ClickIsStaging();
                page.EnterProblemText("TEST PROBLEM TEXT LINE 1\nTEST PROBLEM TEXT LINE 2");
                page.EnterSolutionText("TEST SOLUTION TEXT LINE 1\nTEST SOLUTION TEXT LINE 2");

                string problemTitle = "TEST-2026-001";
                Assert.Equal(problemTitle, page.GetProblemTitle());

                page.ClickSaveButton();

                Assert.True(_wait.Until(d => d.Url.Contains($"/problem/edit/{problemTitle}")));

                // rollback
                await DeleteCall($"/problem/{problemTitle}");

                Assert.Equal(problemTitle, page.GetProblemTitle());
                Assert.Equal("TEST", page.GetProblemCategory());
                Assert.Equal("A", page.GetProblemAnswer());
                Assert.Equal("Test", page.GetProblemTags());
                Assert.Equal("A,B,C,D,E", page.GetAnswerOptions());
                Assert.Equal(false, page.GetIsStaging());
                Assert.Equal("TEST PROBLEM TEXT LINE 1\nTEST PROBLEM TEXT LINE 2", (page.GetProblemText() ?? "").Replace("\r\n", "\n"));
                Assert.Equal("TEST SOLUTION TEXT LINE 1<br/>\nTEST SOLUTION TEXT LINE 2<br/>", (page.GetSolutionText() ?? "").Replace("\r\n", "\n"));
                Assert.Equal("TEST PROBLEM TEXT LINE 1 TEST PROBLEM TEXT LINE 2", page.GetProblemTextBase64());

            });
        }

        [Fact]
        public async Task User_Updates_Problem()
        {
            await RunTestAsync(async () =>
            {
                string problemTitle = "TEST-2026-002";

                var page = new ProblemEditPage(_driver);
                page.GoTo(problemTitle);

                page.EnterProblemAnswer("E");
                page.EnterProblemTags("PL,Test");
                page.ClickIsStaging();
                page.EnterProblemText("UPDATE PROBLEM TEXT LINE 1\nUPDATE PROBLEM TEXT LINE 2");
                page.EnterSolutionText("UPDATE SOLUTION TEXT LINE 1\nUPDATE SOLUTION TEXT LINE 2");
                page.ClickSaveButton();

                // get data from service and assert
                var data = await GetCall<ProblemInfo>($"problem/{problemTitle}");

                // rollback               
                var res = await PutCall($"problem/{problemTitle}", ProblemTestData.RollbackUpdatePayload);

                Assert.Equal(problemTitle, data!.ProblemTitle);
                Assert.Equal("PL,Test", String.Join(',', data.ProblemTags!));
                Assert.Equal(false, data.IsStaging);
                Assert.Equal("UPDATE PROBLEM TEXT LINE 1\nUPDATE PROBLEM TEXT LINE 2", data.ProblemText);
                Assert.Equal("UPDATE SOLUTION TEXT LINE 1<br/>\r\nUPDATE SOLUTION TEXT LINE 2<br/>", data.SolutionText);

            });
        }

        [Fact]
        public void User_View_Problem()
        {
            RunTest(() =>
            {
                string problemTitle = "TEST-2026-003";
                var page = new ProblemViewPage(_driver);
                page.GoTo(problemTitle);

                Assert.Equal("TEST", page.GetProblemCategory());
                Assert.Equal("2026", page.GetProblemYear());
                Assert.Equal("003", page.GetProblemNumber());
                Assert.Equal("A,B,C,D,E", page.GetAnswerOptions());
                Assert.Equal("PL", page.GetProblemTags());
                Assert.Equal("Yes", page.GetIsStaging());
                Assert.Equal("Test Problem Text", page.GetProblemText());
                Assert.True(page.ClickProblemAnswer());
                Assert.Equal("D", page.GetProblemAnswer());
                Assert.True(page.ClickSolutionText());
                Assert.Equal("No Solution", page.GetSolutionText());
                Assert.Contains("TEST-2026-002", page.GetPreviousProblem());
                Assert.Contains("TEST-2026-004", page.GetNextProblem());

                page.ClickEditButton();
                Assert.True(_wait.Until(d => d.Url.Contains($"/problem/edit/{problemTitle}")));
            });
        }

        [Fact]
        public void User_Sees_Math_Problem()
        {
            RunTest(() =>
            {
                string problemTitle = "AMC10-2015A-019";
                var page = new ProblemViewPage(_driver);
                page.GoTo(problemTitle);

                var screenshotFile = page.TakeSnapshotOfProblemText();
                Assert.NotNull(screenshotFile);
                var result = CommonHelper.ComparePictureWithTolerance("problem_view_AMC10-2015A-019", screenshotFile);
                Assert.True(result);
            });
        }

    }
}