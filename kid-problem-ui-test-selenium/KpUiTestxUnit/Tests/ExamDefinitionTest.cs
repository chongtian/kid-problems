using KpUiTestxUnit.Data;
using KpUiTestxUnit.Models;
using KpUiTestxUnit.Pages;
using KpUiTestxUnit.Utilties;
using OpenQA.Selenium.DevTools.V142.Debugger;

namespace KpUiTestxUnit.Tests
{
    public class ExamDefinitionTest : TestBase
    {
        public static IEnumerable<object?[]> DataForCreateExamDefinition()
        {
            yield return new object?[] { TestDataExamDefinition.CreateExamDefinition1 };
            yield return new object?[] { TestDataExamDefinition.CreateExamDefinition2 };
        }

        public static IEnumerable<object?[]> DataForViewExamDefinition()
        {
            yield return new object?[] { TestDataExamDefinition.ViewExamDefinition1 };
            yield return new object?[] { TestDataExamDefinition.ViewExamDefinition2 };
        }

        public ExamDefinitionTest(SetupFixture fixture) : base(fixture, true)
        {

        }

        [Theory]
        [MemberData(nameof(DataForCreateExamDefinition))]
        public async Task User_Create_Exam_Definition(ExamDefinition testData)
        {
            await RunTestAsync(async () =>
                    {
                        var page = new ExamDefEditPage(_driver);
                        page.GoTo();

                        page.SelectExamCategory(testData.ExamCategory);
                        page.EnterExamTitle(testData.ExamTitle);
                        page.EnterExamYear(testData.ExamYear!);
                        page.EnterMemo(testData.Memo!);
                        page.SelectExamType(testData.ExamType);
                        page.ClickAddProblemButton();
                        page.QueryPage.ClickProblemCheckboxes(testData.ProblemTitleSelectIndexes!);
                        page.QueryPage.ClickSelectButton();

                        var details = page.GetExamDetails();
                        Assert.True(details.Length == testData.ProblemTitles!.Length);
                        for (int i = 0; i < testData.ProblemTitles!.Length; i++)
                        {
                            Assert.Equal(testData.ProblemTitles![i], details[i]);
                        }

                        Assert.True(page.GetExamStatus() == testData.ActiveStatusValue);

                        page.ClickSaveButton();
                        string examDefTitleEncoded = CommonHelper.EncodeUrl($"{testData.ExamCategory}/{testData.ExamTitle}");
                        Assert.True(_wait.Until(d => d.Url.Contains($"/examdef/edit/{examDefTitleEncoded}")));

                        // rollback
                        await DeleteCall($"/examdef/{examDefTitleEncoded}");
                    });
        }


        [Theory]
        [MemberData(nameof(DataForViewExamDefinition))]
        public void User_Views_Exam_Definition(ExamDefinition testData)
        {
            RunTest(() =>
            {
                string examDefId = CommonHelper.EncodeUrl($"{testData.ExamCategory}/{testData.ExamTitle}");
                var page = new ExamDefViewPage(_driver);
                page.GoTo(examDefId);

                Assert.Equal(testData.ExamTitle, page.GetExamTitle());
                Assert.Equal(testData.ExamCategory, page.GetExamCategory());
                Assert.Equal(testData.ExamYear, page.GetExamYear());
                Assert.Equal(testData.ExamType, page.GetExamType());
                Assert.Equal(testData.Memo, page.GetMemo() ?? "");
                Assert.Equal(testData.ActiveStatusText, page.GetActiveFlag());
                var details = page.GetProblemTitlesFromExamDetails();
                Assert.True(details.Length == testData.ProblemTitles!.Length);
                for (int i = 0; i < testData.ProblemTitles!.Length; i++)
                {
                    Assert.Equal(testData.ProblemTitles![i], details[i]);
                }

                page.ClickEditButton();
                Assert.Equal("Edit Exam Definition", page.GetPageTitle());
            });

        }

        [Theory]
        [InlineData("AMC10", "TestData20260213001")]
        public async Task User_Create_Assignment_From_Exam_Definition(string examCategory, string examTitle)
        {
            await RunTestAsync(async () =>
            {
                string testExamDefTitle = CommonHelper.EncodeUrl($"{examCategory}/{examTitle}");
                var page = new ExamDefViewPage(_driver);
                page.GoTo(testExamDefTitle);

                page.ClickCreateAssignmentButton();
                Assert.True(_wait.Until(d => d.Url.Contains("/assignment/view/")));

                var destPage = new AssignmentViewPage(_driver);
                Assert.Equal(examTitle, destPage.GetExamTitle());
                Assert.Equal(examCategory, destPage.GetExamCategory());
                Assert.NotNull(destPage.GetCreateTime());
                Assert.Equal("No", destPage.GetCompleted());

                // rollback
                string assignmentId = _driver.Url.Split('/').Last();
                await DeleteCall($"/assignment/{assignmentId}");

            });
        }

        [Fact]
        public void User_Inactivate_Exam_Def()
        {
            RunTest(() =>
            {
                string testExamDefTitle = "AMC10/TestData20260210002";
                var page = new ExamDefEditPage(_driver);
                page.GoTo(testExamDefTitle);
                
                var currStatus = page.GetExamStatus();
                Assert.NotNull(currStatus);

                page.ClickExamStatus();
                page.ClickSaveButton();

                var viewPage = new ExamDefViewPage(_driver);
                viewPage.GoTo(testExamDefTitle);
                bool active = viewPage.GetActiveFlag() == "Yes" ? true : false;
                Assert.True(active == !currStatus.Value);
            });
        }
    }
}
