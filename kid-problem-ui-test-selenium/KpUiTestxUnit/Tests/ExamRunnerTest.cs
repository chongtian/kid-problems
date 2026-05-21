using System.Threading.Tasks;
using KpUiTestxUnit.Data;
using KpUiTestxUnit.Models;
using KpUiTestxUnit.Pages;
using KpUiTestxUnit.Utilties;

namespace KpUiTestxUnit.Tests
{
    public class ExamRunnerTest : TestBase
    {
        public static IEnumerable<object?[]> DataForExamRun()
        {
            yield return new object?[] { TestDataExamRun.Test1 };
        }

        public ExamRunnerTest(SetupFixture fixture) : base(fixture, false)
        {

        }

        [Fact]
        public async Task Child_Clicks_Answer_Button()
        {
            await RunTestAsync(async () =>
            {
                // set up test data
                var testData = TestDataExamRun.ChildClickANswerButton;
                foreach (var detail in testData)
                {
                    await PutCall($"/examrun/detail/{detail.Id}", detail);
                }

                string runId = "adb1a29f-67c5-4606-bc22-d7e807672cc9";
                var page = new ExamRunnerPage(_driver);
                page.GoTo(runId);

                Assert.Equal("Problem 1 - AMC10-2010A-001", page.GetCurrentProblemTitle());

                page.ClickAnswerButton("A");
                Assert.Equal("rgb(255, 0, 0)".RgbaToHex(), page.GetAnswerButtonColor("A").RgbaToHex());

                page.ClickAnswerButton("C");
                Assert.Equal("rgb(63, 81, 181)".RgbaToHex(), page.GetAnswerButtonColor("A").RgbaToHex());
                Assert.Equal("rgb(255, 0, 0)".RgbaToHex(), page.GetAnswerButtonColor("C").RgbaToHex());

                Assert.Equal("rgb(144, 238, 144)".RgbaToHex(), page.GetNavigateButtonColor("1").RgbaToHex());

                page.ClickDeleteAnswerButton();
                Assert.Equal("rgb(63, 81, 181)".RgbaToHex(), page.GetAnswerButtonColor("C").RgbaToHex());

            });
        }

        [Fact]
        public async Task Child_Navigates_Among_Problems()
        {
            await RunTestAsync(async () =>
            {
                // set up test data
                var testData = TestDataExamRun.ChildClickANswerButton;
                foreach (var detail in testData)
                {
                    await PutCall($"/examrun/detail/{detail.Id}", detail);
                }

                string runId = "adb1a29f-67c5-4606-bc22-d7e807672cc9";
                var page = new ExamRunnerPage(_driver);
                page.GoTo(runId);

                Assert.Equal("Problem 1 - AMC10-2010A-001", page.GetCurrentProblemTitle());

                page.ClickAnswerButton("A").ClickNavigateButton("2");
                Assert.Equal("Problem 2 - AMC10-2010A-002", page.GetCurrentProblemTitle());

            });
        }

        [Fact]
        public void Child_Verifies_Summary()
        {
            RunTest(() =>
            {
                string runId = "fa63a4ad-2d4f-40c6-bc62-ed4ecf94ac3f";
                var page = new ExamRunnerPage(_driver);
                page.GoTo(runId);

                Assert.Equal("Problem 1 - AMC10-2020A-001", page.GetCurrentProblemTitle());
                Assert.Equal("rgb(144, 238, 144)".RgbaToHex(), page.GetNavigateButtonColor("1").RgbaToHex());
                Assert.Equal("rgb(144, 238, 144)".RgbaToHex(), page.GetNavigateButtonColor("2").RgbaToHex());
                Assert.Equal("rgb(144, 238, 144)".RgbaToHex(), page.GetNavigateButtonColor("3").RgbaToHex());
                Assert.Equal("rgb(144, 238, 144)".RgbaToHex(), page.GetNavigateButtonColor("4").RgbaToHex());
                Assert.Equal("rgb(255, 255, 0)".RgbaToHex(), page.GetNavigateButtonColor("5").RgbaToHex());

                page.ClickExpandSummaryPanel();
                var summaries = page.GetExamRunDetail();
                Assert.NotNull(summaries);
                Assert.Equal(5, summaries.Length);
                Assert.Equal("AMC10-2020A-001ENo42", summaries[0].ToString());
                Assert.Equal("AMC10-2020A-002CNo29", summaries[1].ToString());
                Assert.Equal("AMC10-2020A-003ANo20", summaries[2].ToString());
                Assert.Equal("AMC10-2020A-004EYes26", summaries[3].ToString());
                Assert.Equal("AMC10-2020A-005No", summaries[4].ToString());

            });
        }

        [Fact]
        public void Child_Clicks_Need_More_Time()
        {
            RunTest(() =>
            {
                string runId = "af27a24b-6a90-4b79-bed6-78edc74ee3c0";
                var page = new ExamRunnerPage(_driver);
                page.GoTo(runId).ClickCompleteButton();
                Assert.True(page.IsReviewButtonsShown());

                page.ClickNotCompleteButton();
                Assert.True(page.IsReviewButtonsHidden());

            });
        }

        [Fact]
        public void Child_Can_Do_Assignment()
        {
            RunTest(() =>
            {
                // a long and slow test

                var assignmentListPage = new AssignmentListPage(_driver);
                assignmentListPage.GoTo(true)
                .SetQueryDateRange("3/1/2026", "3/3/2026")
                .ClickSearchButton()
                .ClickDoAssignmentButtonInQueryResults(0);
                Assert.True(_wait.Until(d => d.Url.Contains("/examrun/run/")));

                var runnerPage = new ExamRunnerPage(_driver);
                Assert.Equal("Problem 1 - AMC10-2010A-001", runnerPage.GetCurrentProblemTitle());
                runnerPage.ClickAnswerButton("A")
                .ClickSubmitButton()
                .ClickAnswerButton("B")
                .ClickNavigateButton("3")
                .ClickAnswerButton("C")
                .ClickSubmitButton()
                .ClickAnswerButton("D")
                .ClickGuessButton()
                .ClickNavigateButton("5")
                .ClickAnswerButton("E")
                .ClickCompleteButton()
                .ClickFinalCompleteButton();

                var viewPage = new ExamRunViewPage(_driver);
                Assert.Equal("AMC10", viewPage.GetExamCategory());
                Assert.Equal("TestData20260302001", viewPage.GetExamTitle());
                Assert.Equal("Yinkai Gao", viewPage.GetAnsweredBy());
                Assert.Equal("5", viewPage.GetTotalCount());
                Assert.Equal("2", viewPage.GetCorrectCount());
                Assert.Equal("1", viewPage.GetGuessCount());
                Assert.Equal("0", viewPage.GetGuessCorrectCount());
                Assert.Equal(5, viewPage.GetExamRunDetails().Length);

            });
        }

    }
}
