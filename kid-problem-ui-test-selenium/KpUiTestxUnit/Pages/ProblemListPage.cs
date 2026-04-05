using OpenQA.Selenium;

namespace KpUiTestxUnit.Pages;

public sealed class ProblemListPage : BasePage
{

    private readonly string rootCssSelector = "app-problem-query";

    public ProblemListPage(IWebDriver driver) : base(driver)
    { }

    public void GoTo(bool isStaging = false)
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
    }

    public void EnterKeyword(string keyword)
    {
        IsNotLoading();
        var textbox = _wait.Until(d => d.FindElement(By.CssSelector($"{rootCssSelector} input[data-testid=\"keyword\"]")));
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
        var button = _wait.Until(d => d.FindElement(By.CssSelector($"{rootCssSelector} button[data-testid=\"btnSearch\"]")));
        if (button != null && button.Enabled && button.Displayed)
        {
            button.Click();
            IsNotLoading();
        }
    }

    public bool IsLoadMoreButtonShown()
    {
        IsNotLoading();
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

    public void ClickLoadMoreButton()
    {
        IsNotLoading();
        var button = _wait.Until(d => d.FindElement(By.CssSelector($"{rootCssSelector} button[data-testid=\"btnMore\"]")));
        if (button != null && button.Enabled && button.Displayed)
        {
            button.Click();
            IsNotLoading();
        }
    }

    public string[] GetProblemTitle()
    {
        IsNotLoading();
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
