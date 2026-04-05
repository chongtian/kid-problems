using KpUiTestxUnit.Models;
using KpUiTestxUnit.Utilties;
using OpenQA.Selenium;

namespace KpUiTestxUnit.Pages;

public sealed class ExamRunQueryPage : BasePage
{
    public ExamRunQueryPage(IWebDriver driver) : base(driver, "app-exam-run-query")
    { }

    public void GoTo(bool all = true)
    {
        if (all)
        {
            _driver.Navigate().GoToUrl($"{Constants.BASE_URL}/examruns/all");
        }
        else
        {
            _driver.Navigate().GoToUrl($"{Constants.BASE_URL}/examruns");
        }
        IsNotLoading();
    }

    public void SetQueryDateRange(string startTimeText, string endTimeText)
    {
        IsNotLoading();

        var startTimeElement = _wait.Until(d => d.FindElement(By.Name("startTime")));
        if (startTimeElement != null)
        {
            startTimeElement.SendKeys(Keys.Control + "a");
            startTimeElement.SendKeys(Keys.Backspace);
            startTimeElement.SendKeys(startTimeText);
        }

        var endTimeElement = _wait.Until(d => d.FindElement(By.Name("endTime")));
        if (endTimeElement != null)
        {
            endTimeElement.SendKeys(Keys.Control + "a");
            endTimeElement.SendKeys(Keys.Backspace);
            endTimeElement.SendKeys(endTimeText);
        }
    }

    public void ClickSearchButton()
    {
        IsNotLoading();
        var button = _wait.Until(d => d.FindElement(By.CssSelector($"{_rootCssSelector} button[data-testid=\"btnSearch\"]")));
        if (button != null && button.Enabled && button.Displayed)
        {
            button.Click();
        }
        IsNotLoading();
    }

    public ExamRun[] GetExamRunsFromQueryResults()
    {
        IsNotLoading();
        var selector = By.CssSelector($"{_rootCssSelector} tbody tr");
        var hasResult = _wait.Until(d => d.FindElements(selector).Count > 0);
        if (hasResult)
        {
            var rows = _wait.Until(d => d.FindElements(selector));
            var ret = new ExamRun[rows.Count];
            for (int i = 0; i < rows.Count; i++)
            {
                var id = rows[i].FindElement(By.CssSelector("td[data-testid=\"examTitle\"] a")).GetAttribute("href")?.Split('/').LastOrDefault() ?? "";
                var examCategory = rows[i].FindElement(By.CssSelector("td[data-testid=\"examCategory\"]")).Text.Trim();
                var examTitle = rows[i].FindElement(By.CssSelector("td[data-testid=\"examTitle\"]")).Text.Trim();
                var totalCount = CommonHelper.ConvertToInt(rows[i].FindElement(By.CssSelector("td[data-testid=\"totalCount\"]")).Text);
                var correctCount = CommonHelper.ConvertToInt(rows[i].FindElement(By.CssSelector("td[data-testid=\"correctCount\"]")).Text);
                var startTime = rows[i].FindElement(By.CssSelector("td[data-testid=\"startTime\"]")).Text.Trim();
                var completeTime = rows[i].FindElement(By.CssSelector("td[data-testid=\"completeTime\"]")).Text.Trim();
                var totalDuration = rows[i].FindElement(By.CssSelector("td[data-testid=\"totalDuration\"]")).Text.Trim();
                var answerBy = rows[i].FindElement(By.CssSelector("td[data-testid=\"answerBy\"]")).Text.Trim();

                ret[i] = new ExamRun
                {
                    UID = id,
                    ExamTitle = examTitle,
                    ExamCategory = examCategory,
                    TotalCount = totalCount,
                    CorrectCount = correctCount,
                    StartTime = startTime,
                    CompleteTime = completeTime,
                    TotalDuration = totalDuration,
                    AnsweredBy = answerBy
                };
            }
            return ret;
        }

        return [];
    }

    public void ClickExamTitleInQueryResults(int index)
    {
        IsNotLoading();
        var selector = By.CssSelector($"{_rootCssSelector} tbody tr");
        var hasResult = _wait.Until(d => d.FindElements(selector).Count > 0);
        if (hasResult)
        {
            var rows = _wait.Until(d => d.FindElements(selector));
            var row = rows.ElementAtOrDefault(index);
            if (row != null)
            {
                var link = row.FindElement(By.CssSelector("td[data-testid=\"examTitle\"] a"));
                if (link != null && link.Enabled && link.Displayed)
                {
                    link.Click();
                    IsNotLoading();
                }
            }
        }
    }

    public string? GetCountOfQueryResults()
    {
        IsNotLoading();

        var countElement = _wait.Until(d => d.FindElement(By.CssSelector($"{_rootCssSelector} div[data-testid=\"countOfRecord\"]")));
        if (countElement != null)
        {
            var countText = countElement.Text;
            if (!string.IsNullOrEmpty(countText) && countText.Contains(':'))
            {
                return countText.Substring(countText.IndexOf(':') + 1).Trim();
            }
        }

        return null; ;
    }

}
