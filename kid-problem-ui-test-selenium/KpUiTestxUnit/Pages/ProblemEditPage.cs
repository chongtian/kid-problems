using OpenQA.Selenium;

namespace KpUiTestxUnit.Pages;

public sealed class ProblemEditPage : BasePage
{

    public ProblemEditPage(IWebDriver driver) : base(driver, "app-problem-detail")
    { }

    public ProblemEditPage GoTo(string? problemTitle = null)
    {
        if (string.IsNullOrEmpty(problemTitle))
        {
            _driver.Navigate().GoToUrl($"{Constants.BASE_URL}/problem/create");
        }
        else
        {
            _driver.Navigate().GoToUrl($"{Constants.BASE_URL}/problem/edit/{problemTitle}");
        }

        IsNotLoading();

        return this;
    }

    // ProblemTitle is readonly
    // public ProblemEditPage EnterProblemTitle(string value)
    // {
    //     EnterTextField("problemTitle", value);
    // }

    public string? GetProblemTitle()
    {
        return GetTextFieldValue("problemTitle");
    }

    public ProblemEditPage EnterProblemCategory(string value)
    {
        EnterTextField("problemCategory", value);
        return this;
    }

    public string? GetProblemCategory()
    {
        return GetTextFieldValue("problemCategory");
    }

    public ProblemEditPage EnterProblemYear(string value)
    {
        EnterTextField("problemYear", value);
        return this;
    }

    public string? GetProblemYear()
    {
        return GetTextFieldValue("problemYear");
    }

    public ProblemEditPage EnterProblemNumber(string value)
    {
        EnterTextField("problemNumber", value);
        return this;
    }

    public string? GetProblemNumber()
    {
        return GetTextFieldValue("problemNumber");
    }

    public ProblemEditPage EnterProblemAnswer(string value)
    {
        EnterTextField("problemAnswer", value);
        return this;
    }

    public string? GetProblemAnswer()
    {
        return GetTextFieldValue("problemAnswer");
    }

    public ProblemEditPage EnterProblemTags(string value)
    {
        EnterTextField("problemTags", value);
        return this;
    }

    public string? GetProblemTags()
    {
        return GetTextFieldValue("problemTags");
    }

    public ProblemEditPage EnterAnswerOptions(string value)
    {
        EnterTextField("answerOptions", value);
        return this;
    }

    public string? GetAnswerOptions()
    {
        return GetTextFieldValue("answerOptions");
    }

    public ProblemEditPage EnterProblemText(string value)
    {
        EnterTextField("problemText", value, true);
        return this;
    }

    public string? GetProblemText()
    {
        return GetTextFieldValue("problemText", true);
    }

    public ProblemEditPage EnterSolutionText(string value)
    {
        EnterTextField("solutionText", value, true);
        return this;
    }

    public string? GetSolutionText()
    {
        return GetTextFieldValue("solutionText", true);
    }

    public ProblemEditPage ClickIsStaging()
    {
        IsNotLoading();
        var checkbox = _wait.Until(d => d.FindElement(By.CssSelector($"{_rootCssSelector} mat-checkbox[data-testid=\"isStaging\"] input[type=\"checkbox\"]")));
        if (checkbox != null)
        {
            checkbox.Click();
        }
        return this;
    }

    public bool? GetIsStaging()
    {
        IsNotLoading();
        var checkbox = _wait.Until(d => d.FindElement(By.CssSelector($"{_rootCssSelector} mat-checkbox[data-testid=\"isStaging\"] input[type=\"checkbox\"]")));
        if (checkbox != null)
        {
            return checkbox.Selected;
        }
        return null;
    }

    public string? GetProblemTextBase64()
    {
        var textField = _wait.Until(d => d.FindElement(By.CssSelector($"{_rootCssSelector} div[data-testid=\"problemRichText\"]")));
        return textField.Text.Trim();
    }

    public ProblemEditPage ClickSaveButton(bool confirm = true)
    {
        IsNotLoading();
        var button = _wait.Until(d => d.FindElement(By.CssSelector($"{_rootCssSelector} button[data-testid=\"btnSave\"]")));
        if (button != null && button.Enabled && button.Displayed)
        {
            button.Click();
            AlertHelper.HandleAlert(_wait, confirm);
            IsNotLoading();
        }
        return this;
    }

    public ProblemEditPage ClickPreviewButton()
    {
        IsNotLoading();
        var button = _wait.Until(d => d.FindElement(By.CssSelector($"{_rootCssSelector} button[data-testid=\"btnPreview\"]")));
        if (button != null && button.Enabled && button.Displayed)
        {
            button.Click();
            IsNotLoading();
        }
        return this;
    }

    public ProblemEditPage ClickDeleteButton(bool confirm = true)
    {
        IsNotLoading();
        var button = _wait.Until(d => d.FindElement(By.CssSelector($"{_rootCssSelector} button[data-testid=\"btnDelete\"]")));
        if (button != null && button.Enabled && button.Displayed)
        {
            button.Click();
            AlertHelper.HandleAlert(_wait, confirm);
            IsNotLoading();
        }
        return this;
    }

}
