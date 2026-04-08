using KpUiTestxUnit.Data;
using KpUiTestxUnit.Models;
using KpUiTestxUnit.Pages;

namespace KpUiTestxUnit.Tests
{
    public class ExamRunTest : TestBase
    {
        public static IEnumerable<object?[]> DataForExamRun()
        {
            yield return new object?[] { TestDataExamRun.Test1 };
        }

        public ExamRunTest(SetupFixture fixture) : base(fixture, true)
        {

        }

        [Theory]
        [InlineData("8/1/2023", "8/15/2023", "12", "AMC10 Review 147 529")]
        [InlineData("9/2/2023", "9/30/2023", "20", "AMC10 Review 177 564")]
        public void User_Query_In_Exam_Summaries(string startTime, string endTime, string expectedCount, string firstRecord)
        {
            RunTest(() =>
            {
                var page = new ExamRunQueryPage(_driver);
                page.GoTo();

                page.SetQueryDateRange(startTime, endTime);
                page.ClickSearchButton();
                Assert.Equal(expectedCount, page.GetCountOfQueryResults());

                var records = page.GetExamRunsFromQueryResults();
                Assert.Contains(firstRecord, records[0].ExamTitle);
            });
        }

        [Theory]
        [InlineData("8/1/2023", "8/15/2023", 9, "5423ac10-eec6-4555-8faa-3096d878018a", "AMC10 Review 140 520")]
        [InlineData("9/2/2023", "9/30/2023", 5, "0ab4af3f-6b3b-4232-9aca-3fdc02a8ded7", "AMC10 Review 172 559")]
        public void User_Navigate_To_Detail_From_List_Exam_Summaries(string startTime, string endTime, int selectIndex, string uid, string examTitle)
        {
            RunTest(() =>
            {
                var page = new ExamRunQueryPage(_driver);
                page.GoTo();

                page.SetQueryDateRange(startTime, endTime);
                page.ClickSearchButton();
                page.ClickExamTitleInQueryResults(selectIndex);

                var destPage = new ExamRunViewPage(_driver);
                Assert.True(_wait.Until(d => d.Url.Contains($"{Constants.BASE_URL}/examrun/view/{uid}")));
                Assert.Equal(examTitle, destPage.GetExamTitle());
            });
        }

        [Theory]
        [MemberData(nameof(DataForExamRun))]
        public void User_View_Exam_Run(ExamRun testData)
        {
            RunTest(() =>
                    {
                        var page = new ExamRunViewPage(_driver);
                        page.GoTo(testData.UID!);

                        Assert.Equal(testData.ExamCategory, page.GetExamCategory());
                        Assert.Equal(testData.ExamTitle, page.GetExamTitle());
                        Assert.Equal(testData.AnsweredBy, page.GetAnsweredBy());
                        Assert.Equal(testData.StartTime, page.GetStartTime());
                        Assert.Equal(testData.CompleteTime, page.GetCompleteTime());
                        Assert.Equal(testData.TotalDuration, page.GetTotalDuration());
                        Assert.Equal(testData.TotalCount.ToString(), page.GetTotalCount());
                        Assert.Equal(testData.CorrectCount.ToString(), page.GetCorrectCount());
                        Assert.Equal(testData.GuessCount.ToString(), page.GetGuessCount());
                        Assert.Equal(testData.GuessCorrectCount.ToString(), page.GetGuessCorrectCount());

                        var details = page.GetExamRunDetails();
                        for (int i = 0; i < testData.ExamDetails!.Length; i++)
                        {
                            Assert.Equal(details[i].ToString(), testData.ExamDetails[i].ToString());
                        }
                    });
        }

        [Theory]
        [InlineData("8f3731de-2e41-42c1-8b47-fbc9e6d9925d", "ac0d1f40-87cc-4ba9-a202-5420dace507f", "August 8, 2023, 9:11:38 AM GMT-5")]
        [InlineData("5b717440-6871-40bb-a2a9-80b471d0de29", "0b583da7-8886-45c3-bba5-b78aee36580b", "August 7, 2023, 9:56:26 AM GMT-5")]
        public void User_Click_Assignment_Link_From_Exam_Run(string examRunUid, string assignmentUid, string assignmentCreateTime)
        {
            RunTest(() =>
                    {
                        var page = new ExamRunViewPage(_driver);
                        page.GoTo(examRunUid);
                        page.ClickGoToAssignment();
                        var destPage = new AssignmentViewPage(_driver);

                        Assert.True(_wait.Until(d => d.Url.Contains($"{Constants.BASE_URL}/assignment/view/{assignmentUid}")));
                        Assert.Equal(assignmentCreateTime, destPage.GetCreateTime());
                    });
        }
    }
}
