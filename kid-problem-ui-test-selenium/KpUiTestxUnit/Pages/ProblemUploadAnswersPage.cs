using OpenQA.Selenium;

namespace KpUiTestxUnit.Pages;

public class ProblemUploadAnswersPage : BasePage
{
    private readonly string rootCssSelector = "app-update-answer";
    public ProblemUploadAnswersPage(IWebDriver driver) : base(driver) { }

    public ProblemUploadAnswersPage GoTo()
    {
        _driver.Navigate().GoToUrl($"{Constants.BASE_URL}/problem/answers");
        return this;
    }

    public ProblemUploadAnswersPage EnterProblemCategory(string value)
    {
        IsNotLoading();
        var textbox = _wait.Until(d => d.FindElement(By.CssSelector($"{rootCssSelector} input[data-testid=\"problemCategory\"]")));
        if (textbox != null && textbox.Enabled && textbox.Displayed)
        {
            textbox.SendKeys(Keys.Control + "a");
            textbox.SendKeys(Keys.Backspace);
            textbox.SendKeys(value);
        }
        return this;
    }

    public ProblemUploadAnswersPage EnterProblemYear(string value)
    {
        IsNotLoading();
        var textbox = _wait.Until(d => d.FindElement(By.CssSelector($"{rootCssSelector} input[data-testid=\"problemYear\"]")));
        if (textbox != null && textbox.Enabled && textbox.Displayed)
        {
            textbox.SendKeys(Keys.Control + "a");
            textbox.SendKeys(Keys.Backspace);
            textbox.SendKeys(value);
        }
        return this;
    }

    public ProblemUploadAnswersPage EnterAnswerKeys(string value)
    {
        IsNotLoading();
        var textbox = _wait.Until(d => d.FindElement(By.CssSelector($"{rootCssSelector} textarea[data-testid=\"answerKeys\"]")));
        if (textbox != null && textbox.Enabled && textbox.Displayed)
        {
            textbox.SendKeys(Keys.Control + "a");
            textbox.SendKeys(Keys.Backspace);
            textbox.SendKeys(value);
        }
        return this;
    }

    public string? GetGeneratedAnswerKeys()
    {
        IsNotLoading();
        var textbox = _wait.Until(d => d.FindElement(By.CssSelector($"{rootCssSelector} textarea[data-testid=\"problemAnswersText\"]")));
        if (textbox != null && textbox.Enabled && textbox.Displayed)
        {
            return textbox.GetAttribute("value");
        }
        return null;
    }

    public ProblemUploadAnswersPage ClickGenerateButton()
    {
        IsNotLoading();
        var button = _wait.Until(d => d.FindElement(By.CssSelector($"{rootCssSelector} button[data-testid=\"btnGenerate\"]")));
        if (button != null && button.Enabled && button.Displayed)
        {
            button.Click();
            IsNotLoading();
        }
        return this;
    }

    public ProblemUploadAnswersPage ClickSaveButton()
    {
        IsNotLoading();
        var button = _wait.Until(d => d.FindElement(By.CssSelector($"{rootCssSelector} button[data-testid=\"btnSave\"]")));
        if (button != null && button.Enabled && button.Displayed)
        {
            button.Click();
            IsNotLoading();
        }
        return this;
    }

}
