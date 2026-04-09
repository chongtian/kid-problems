using KpUiTestxUnit.Models;
using OpenQA.Selenium;

namespace KpUiTestxUnit.Pages;

public sealed class AssignmentListPage : BasePage
{
    public readonly PaginatorComponentPage Paginator;

    public AssignmentListPage(IWebDriver driver) : base(driver, "app-assignment-query")
    {
        Paginator = new PaginatorComponentPage(driver);
    }

    public void GoTo(bool all = true)
    {
        if (all)
        {
            _driver.Navigate().GoToUrl($"{Constants.BASE_URL}/assignments/all");
        }
        else
        {
            _driver.Navigate().GoToUrl($"{Constants.BASE_URL}/assignments");
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

    public Assignment[] GetAssignmentsFromQueryResults()
    {
        IsNotLoading();
        var selector = By.CssSelector($"{_rootCssSelector} tbody tr");
        var hasResult = _wait.Until(d => d.FindElements(selector).Count > 0);
        if (hasResult)
        {
            var rows = _wait.Until(d => d.FindElements(selector));
            var ret = new Assignment[rows.Count];
            for (int i = 0; i < rows.Count; i++)
            {
                var createTime = rows[i].FindElement(By.CssSelector("td[data-testid=\"createTime\"]")).Text.Trim();
                var id = rows[i].FindElement(By.CssSelector("td[data-testid=\"createTime\"] a")).GetAttribute("href")?.Split('/').LastOrDefault() ?? "";
                var examCategory = rows[i].FindElement(By.CssSelector("td[data-testid=\"examCategory\"]")).Text.Trim();
                var examTitle = rows[i].FindElement(By.CssSelector("td[data-testid=\"examTitle\"]")).Text.Trim();
                var isComplete = rows[i].FindElement(By.CssSelector("td[data-testid=\"isComplete\"]")).Text.Trim();
                var memo = rows[i].FindElement(By.CssSelector("td[data-testid=\"memo\"]")).Text.Trim();

                ret[i] = new Assignment
                {
                    UID = id,
                    ExamTitle = examTitle,
                    ExamCategory = examCategory,
                    CreateTime = createTime,
                    Completed = isComplete,
                    Memo = memo
                };
            }
            return ret;
        }

        return [];
    }

    public void ClickAssignmentTitleInQueryResults(int index)
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
                var link = row.FindElement(By.CssSelector("td[data-testid=\"createTime\"] a"));
                if (link != null && link.Enabled && link.Displayed)
                {
                    link.Click();
                    IsNotLoading();
                }
            }
        }
    }

    public void ClickDoAssignmentButtonInQueryResults(int index)
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
                var button = row.FindElement(By.CssSelector("button[data-testid=\"btnDoAssignment\"]"));
                if (button != null && button.Enabled && button.Displayed)
                {
                    button.Click();
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

    public bool IsLoadMoreButtonShown()
    {
        IsNotLoading();
        return _wait.Until(d => d.FindElements(By.CssSelector($"{_rootCssSelector} button[data-testid=\"btnMore\"]")).Count > 0);
    }

    public bool IsLoadMoreButtonHidden()
    {
        IsNotLoading();
        return _wait.Until(d => d.FindElements(By.CssSelector($"{_rootCssSelector} button[data-testid=\"btnMore\"]")).Count == 0);
    }

    public void ClickMoreButton()
    {
        IsNotLoading();
        var button = _wait.Until(d => d.FindElement(By.CssSelector($"{_rootCssSelector} button[data-testid=\"btnMore\"]")));
        if (button != null && button.Displayed && button.Enabled)
        {
            button.Click();
            IsNotLoading();
        }
    }

}
