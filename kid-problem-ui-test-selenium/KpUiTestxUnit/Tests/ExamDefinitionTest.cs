using KpUiTestxUnit.Data;
using KpUiTestxUnit.Models;
using KpUiTestxUnit.Pages;
using KpUiTestxUnit.Utilties;

namespace KpUiTestxUnit.Tests
{
    public class ExamDefinitionTest : TestBase
    {
        public static IEnumerable<object[]> DataForCreateExamDefinition()
        {
            yield return new object[] { TestDataExamDefinition.CreateExamDefinition1 };
            yield return new object[] { TestDataExamDefinition.CreateExamDefinition2 };
        }

        public static IEnumerable<object[]> DataForViewExamDefinition()
        {
            yield return new object[] { TestDataExamDefinition.ViewExamDefinition1 };
            yield return new object[] { TestDataExamDefinition.ViewExamDefinition2 };
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
                        page.GoTo()
                        .SelectExamCategory(testData.ExamCategory!)
                        .EnterExamTitle(testData.ExamTitle!)
                        .EnterExamYear(testData.ExamYear!)
                        .EnterMemo(testData.Memo!)
                        .SelectExamType(testData.ExamType!)
                        .ClickAddProblemButton();

                        page.QueryPage
                        .ClickProblemCheckboxes(testData.ProblemTitleSelectIndexes!)
                        .ClickSelectButton();

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
                page.GoTo(testExamDefTitle)
                .ClickCreateAssignmentButton();
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

                page.ClickExamStatus().ClickSaveButton();

                var viewPage = new ExamDefViewPage(_driver);
                viewPage.GoTo(testExamDefTitle);
                bool active = viewPage.GetActiveFlag() == "Yes" ? true : false;
                Assert.True(active == !currStatus.Value);
            });
        }

        [Fact]
        public void User_Queries_Exam_Definitions()
        {
            RunTest(() =>
            {
                var page = new ExamDefinitionListPage(_driver);
                page.GoTo()
                .SelectExamCategory("AMC8")
                .EnterKeyword("AMC8-201")
                .ClickSearchButton();

                Assert.Equal("25", page.GetCountOfQueryResults());

                page.Paginator.ClickNextPageButton();
                var records = page.GetExamDefinitionsFromQueryResults();
                Assert.Equal(10, records.Length);
                Assert.Contains("AMC8-2013 Half 1 138", records[9].ExamTitle);

                page.Paginator.ClickLastPageButton();
                records = page.GetExamDefinitionsFromQueryResults();
                Assert.Equal(5, records.Length);
                Assert.Contains("AMC8-2013 Half 2 139", records[0].ExamTitle);

                page.Paginator.ClickPreviousPageButton();
                records = page.GetExamDefinitionsFromQueryResults();
                Assert.Equal(10, records.Length);
                Assert.Contains("AMC8-2013 Half 1 138", records[9].ExamTitle);

                page.Paginator.ClickFirstPageButton();
                records = page.GetExamDefinitionsFromQueryResults();
                Assert.Equal(10, records.Length);
                Assert.Contains("AMC8-2010 27", records[0].ExamTitle);
            });
        }

        [Fact]
        public void User_Load_More_ExamDefinitions()
        {
            RunTest(() =>
            {
                var page = new ExamDefinitionListPage(_driver);
                page.GoTo()
                .SelectExamCategory("AMC8")
                .EnterKeyword("AMC8-201")
                .ClickSearchButton();

                Assert.Equal("25", page.GetCountOfQueryResults());
                Assert.True(page.IsLoadMoreButtonShown());

                page.ClickMoreButton();
                Assert.Equal("50", page.GetCountOfQueryResults());
                Assert.True(page.IsLoadMoreButtonShown());

                page.Paginator.ClickLastPageButton();
                var records = page.GetExamDefinitionsFromQueryResults();
                Assert.Equal(10, records.Length);
                Assert.Contains("AMC8-2016-Part 3 90", records[9].ExamTitle);

            });
        }

        [Fact]
        public void User_Navigate_To_Detail_From_List_ExamDefinitions()
        {
            RunTest(() =>
            {
                var page = new ExamDefinitionListPage(_driver);
                page.GoTo()
                .SelectExamCategory("AMC8")
                .EnterKeyword("AMC8-201")
                .ClickSearchButton()
                .ClickExamTitleInQueryResults(0);
                Assert.True(_wait.Until(d => d.Url.Contains($"{Constants.BASE_URL}/examdef/view/AMC8/AMC8-2010%2027")));
                var destPage = new ExamDefViewPage(_driver);
                Assert.Equal("AMC8-2010 27", destPage.GetExamTitle());
            });
        }

    }
}
