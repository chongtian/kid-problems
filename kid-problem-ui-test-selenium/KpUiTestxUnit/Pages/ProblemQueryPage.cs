using OpenQA.Selenium;

namespace KpUiTestxUnit.Pages;

public sealed class ProblemQueryPage : BasePage
{

    private readonly string rootCssSelector = "app-problem-query";

    public ProblemQueryPage(IWebDriver driver) : base(driver)
    { }

    public ProblemQueryPage GoTo(bool isStaging = false)
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

    public ProblemQueryPage EnterKeyword(string keyword)
    {
        var textbox = _wait.Until(d => d.FindElement(By.CssSelector($"{rootCssSelector} input[data-testid=\"keyword\"]")));

        if (textbox != null && textbox.Enabled && textbox.Displayed)
        {
            textbox.SendKeys(Keys.Control + "a");
            textbox.SendKeys(Keys.Backspace);
            textbox.SendKeys(keyword);
        }

        return this;
    }

    public ProblemQueryPage ClickSearchButton()
    {
        var button = _wait.Until(d => d.FindElement(By.CssSelector($"{rootCssSelector} button[data-testid=\"btnSearch\"]")));

        if (button != null && button.Enabled && button.Displayed)
        {
            button.Click();
            IsNotLoading();
        }

        return this;
    }

    public bool IsLoadMoreButtonShown()
    {
        var shortWait = WebDriverUtility.GetShortWait(_driver, 500);
        try
        {
            var button = _wait.Until(d => d.FindElement(By.CssSelector($"{rootCssSelector} button[data-testid=\"btnMore\"]")));
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

    public ProblemQueryPage ClickLoadMoreButton()
    {
        var button = _wait.Until(d => d.FindElement(By.CssSelector($"{rootCssSelector} button[data-testid=\"btnMore\"]")));
        if (button != null && button.Enabled && button.Displayed)
        {
            button.Click();
            IsNotLoading();
        }

        return this;
    }

    public string[] GetProblemTitle()
    {
        var returnedProblems = _wait.Until(d => d.FindElements(By.CssSelector($"{rootCssSelector} span[data-testid=\"problemTitle\"]")));
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
