using OpenQA.Selenium;

namespace KpUiTestxUnit.Pages;

public sealed class ProblemBulkUpdatePage : BasePage
{
    private readonly string rootCssSelector = "app-problem-bulk-update";
    public QueryProblemsPage QueryPage { get; private set; }

    public ProblemBulkUpdatePage(IWebDriver driver) : base(driver)
    {
        QueryPage = new QueryProblemsPage(driver);
    }

    public void GoTo()
    {
        _driver.Navigate().GoToUrl($"{Constants.BASE_URL}/problem/bulkupdate");
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

    public void ClickOpenQueryButton()
    {
        IsNotLoading();

        // this is a mistake in the source code of client app; 
        // data-testid was added to <i> instead of <button>
        // var button = _wait.Until(d => d.FindElement(By.CssSelector($"{rootCssSelector} button[data-testid=\"btnOpenQuery\"]")));
        var button = _wait.Until(d => d.FindElement(By.CssSelector($"{rootCssSelector} button")));
        if (button != null && button.Enabled && button.Displayed)
        {
            button.Click();
            IsNotLoading();
        }
    }

    public void ClickSaveButton()
    {
        IsNotLoading();
        var button = _wait.Until(d => d.FindElement(By.CssSelector($"{rootCssSelector} button[data-testid=\"btnSave\"]")));
        if (button != null && button.Enabled && button.Displayed)
        {
            button.Click();
            IsNotLoading();
        }
    }

    public string[] GetStagingProblemTitles()
    {
        return GetTextFields("problemTitle");
    }

    public string[] GetStagingProblemAnswers()
    {
        return GetTextFields("problemAnswer");
    }

    public void ClickDeleteButton(int index)
    {
        IsNotLoading();
        var button = _wait.Until(d => d.FindElements(By.CssSelector($"{rootCssSelector} span[data-testid=\"btnDelete\"]")).ElementAt(index));
        if (button != null && button.Enabled && button.Displayed)
        {
            button.Click();
            IsNotLoading();
        }
    }

    private string[] GetTextFields(string testId)
    {
        IsNotLoading();
        _wait.Until(d => d.FindElements(By.CssSelector($"{rootCssSelector} span[data-testid=\"{testId}\"]")).Count > 0);
        var textFields = _wait.Until(d => d.FindElements(By.CssSelector($"{rootCssSelector} span[data-testid=\"{testId}\"]")));
        if (textFields != null && textFields.Count > 0)
        {
            int cnt = textFields.Count;
            var ret = new string[cnt];
            for (int i = 0; i < cnt; i++)
            {
                ret[i] = textFields[i].Text.Trim();
            }
            return ret;
        }
        return [];
    }

}
