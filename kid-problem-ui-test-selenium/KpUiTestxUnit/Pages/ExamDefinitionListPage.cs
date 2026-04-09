using KpUiTestxUnit.Models;
using KpUiTestxUnit.Utilties;
using OpenQA.Selenium;

namespace KpUiTestxUnit.Pages;

public sealed class ExamDefinitionListPage : BasePage
{
    public readonly PaginatorComponentPage Paginator;

    public ExamDefinitionListPage(IWebDriver driver) : base(driver, "app-exam-def-query")
    {
        Paginator = new PaginatorComponentPage(driver);
    }

    public void GoTo(bool all = true)
    {
        if (all)
        {
            _driver.Navigate().GoToUrl($"{Constants.BASE_URL}/examdefs/all");
        }
        else
        {
            _driver.Navigate().GoToUrl($"{Constants.BASE_URL}/examdefs");
        }
        IsNotLoading();
    }

    public void EnterKeyword(string keyword)
    {
        IsNotLoading();
        var textbox = _wait.Until(d => d.FindElement(By.CssSelector($"{_rootCssSelector} input[data-testid=\"keyword\"]")));
        if (textbox != null && textbox.Enabled && textbox.Displayed)
        {
            textbox.SendKeys(Keys.Control + "a");
            textbox.SendKeys(Keys.Backspace);
            textbox.SendKeys(keyword);
        }
    }

    public void ClickSearchButton()
    {
        IsNotLoading();
        var button = _wait.Until(d => d.FindElement(By.CssSelector($"{_rootCssSelector} button[data-testid=\"btnSearch\"]")));
        if (button != null && button.Enabled && button.Displayed)
        {
            button.Click();
            IsNotLoading();
        }
    }

    public void SelectExamCategory(string value)
    {
        IsNotLoading();
        var select = _wait.Until(d => d.FindElement(By.CssSelector($"{_rootCssSelector} mat-select[data-testid=\"category\"]")));
        if (select != null && select.Displayed && select.Enabled)
        {
            MatSelectHelper.SelectOptionByLabelValue(_driver, _wait, select, value);
        }
    }

    public string GetExamCategory()
    {
        IsNotLoading();
        var select = _wait.Until(d => d.FindElement(By.CssSelector($"{_rootCssSelector} mat-select[data-testid=\"category\"]")));
        return select.Text;
    }

    public ExamDefinition[] GetExamDefinitionsFromQueryResults()
    {
        IsNotLoading();
        var selector = By.CssSelector($"{_rootCssSelector} tbody tr");
        var hasResult = _wait.Until(d => d.FindElements(selector).Count > 0);
        if (hasResult)
        {
            var rows = _wait.Until(d => d.FindElements(selector));
            var ret = new ExamDefinition[rows.Count];
            for (int i = 0; i < rows.Count; i++)
            {
                var examCategory = rows[i].FindElement(By.CssSelector("td[data-testid=\"examCategory\"]")).Text.Trim();
                var examYear = rows[i].FindElement(By.CssSelector("td[data-testid=\"examYear\"]")).Text.Trim();
                var examTitle = rows[i].FindElement(By.CssSelector("td[data-testid=\"examTitle\"]")).Text.Trim();
                var examType = rows[i].FindElement(By.CssSelector("td[data-testid=\"examType\"]")).Text.Trim();
                var countOfExpectedProblems = CommonHelper.ConvertToInt(rows[i].FindElement(By.CssSelector("td[data-testid=\"countOfProblems\"]")).Text);
                var examStatus = rows[i].FindElement(By.CssSelector("td[data-testid=\"examStatus\"]")).Text.Trim();
                var memo = rows[i].FindElement(By.CssSelector("td[data-testid=\"memo\"]")).Text.Trim();

                ret[i] = new ExamDefinition
                {
                    ExamTitle = examTitle,
                    ExamCategory = examCategory,
                    ExamYear = examYear,
                    ExamType = examType,
                    CountOfExpectedProblems = countOfExpectedProblems,
                    ActiveStatusText = examStatus,
                    Memo = memo
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
