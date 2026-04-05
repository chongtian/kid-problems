using OpenQA.Selenium;

namespace KpUiTestxUnit.Pages;

public sealed class QueryProblemsPage : BasePage
{

    private readonly string rootCssSelector = "app-problem-search-dialog";

    public QueryProblemsPage(IWebDriver driver) : base(driver)
    { }

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

    public void ClickSelectButton()
    {
        IsNotLoading();
        var button = _wait.Until(d => d.FindElement(By.CssSelector($"{rootCssSelector} button[data-testid=\"btnSelect\"]")));
        if (button != null && button.Enabled && button.Displayed)
        {
            button.Click();
            IsNotLoading();
        }
    }

    public void ClickCloseButton()
    {
        IsNotLoading();
        var button = _wait.Until(d => d.FindElement(By.CssSelector($"{rootCssSelector} button[data-testid=\"btnClose\"]")));
        if (button != null && button.Enabled && button.Displayed)
        {
            button.Click();
            IsNotLoading();
        }
    }

    public void ClickSelectAllButton()
    {
        IsNotLoading();
        var button = _wait.Until(d => d.FindElement(By.CssSelector($"{rootCssSelector} mat-slide-toggle[data-testid=\"btnSelectAll\"]")));
        if (button != null && button.Enabled && button.Displayed)
        {
            button.Click();
            IsNotLoading();
        }
    }

    public void ClickProblemCheckboxes(int[] indexes)
    {
        IsNotLoading();
        _wait.Until(d => d.FindElements(By.CssSelector($"{rootCssSelector} mat-checkbox[data-testid=\"problemTitle\"]")).Count > 0);

        foreach (int i in indexes)
        {
            var checkbox = _wait.Until(d => d.FindElements(By.CssSelector($"{rootCssSelector} mat-checkbox[data-testid=\"problemTitle\"]")).ElementAt(i));
            if (checkbox != null && checkbox.Enabled && checkbox.Displayed)
            {
                checkbox.Click();
            }
        }
    }

    public string[] GetProblemTitle()
    {
        IsNotLoading();
        var returnedProblems = _wait.Until(d => d.FindElements(By.CssSelector($"{rootCssSelector} mat-checkbox[data-testid=\"problemTitle\"]")));
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
