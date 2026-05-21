using OpenQA.Selenium;

namespace KpUiTestxUnit.Pages;

public sealed class ProblemListPage : BasePage
{

    public ProblemListPage(IWebDriver driver) : base(driver, "app-problem-query")
    { }

    public ProblemListPage GoTo(bool isStaging = false)
    {
        if (isStaging)
        {
            _driver.Navigate().GoToUrl($"{Constants.BASE_URL}/problems/s");
        }
        else
        {
            _driver.Navigate().GoToUrl($"{Constants.BASE_URL}/problems/r");
        }

        IsNotLoading();

        return this;
    }

    public ProblemListPage EnterKeyword(string keyword)
    {
        IsNotLoading();
        var textbox = _wait.Until(d => d.FindElement(By.CssSelector($"{_rootCssSelector} input[data-testid=\"keyword\"]")));
        if (textbox != null && textbox.Enabled && textbox.Displayed)
        {
            textbox.SendKeys(Keys.Control + "a");
            textbox.SendKeys(Keys.Backspace);
            textbox.SendKeys(keyword);
        }

        return this;
    }

    public ProblemListPage ClickSearchButton()
    {
        IsNotLoading();
        var button = _wait.Until(d => d.FindElement(By.CssSelector($"{_rootCssSelector} button[data-testid=\"btnSearch\"]")));
        if (button != null && button.Enabled && button.Displayed)
        {
            button.Click();
            IsNotLoading();
        }

        return this;
    }

    public bool IsLoadMoreButtonShown()
    {
        IsNotLoading();
        var shortWait = WebDriverUtility.GetShortWait(_driver, 500);
        try
        {
            var button = _wait.Until(d => d.FindElement(By.CssSelector($"{_rootCssSelector} button[data-testid=\"btnMore\"]")));
            return button != null;
        }
        catch (NoSuchElementException)
        {
            return false;
        }
        catch (WebDriverTimeoutException)
        {
            return false;
        }
    }

    public ProblemListPage ClickLoadMoreButton()
    {
        IsNotLoading();
        var button = _wait.Until(d => d.FindElement(By.CssSelector($"{_rootCssSelector} button[data-testid=\"btnMore\"]")));
        if (button != null && button.Enabled && button.Displayed)
        {
            button.Click();
            IsNotLoading();
        }

        return this;
    }

    public string[] GetProblemTitle()
    {
        IsNotLoading();
        var returnedProblems = _wait.Until(d => d.FindElements(By.CssSelector($"{_rootCssSelector} span[data-testid=\"problemTitle\"]")));
        if (returnedProblems != null && returnedProblems.Count > 0)
        {
            string[] ret = new string[returnedProblems.Count];
            for (int i = 0; i < returnedProblems.Count; i++)
            {
                ret[i] = returnedProblems[i].Text.Trim();
            }
            return ret;
        }
        return [];
    }

}
