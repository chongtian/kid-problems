using OpenQA.Selenium;

namespace KpUiTestxUnit.Pages;

public sealed class ProblemEditPage : BasePage
{

    public ProblemEditPage(IWebDriver driver) : base(driver, "app-problem-detail")
    { }

    public void GoTo(string? problemTitle = null)
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
    }

    // ProblemTitle is readonly
    // public void EnterProblemTitle(string value)
    // {
    //     EnterTextField("problemTitle", value);
    // }

    public string? GetProblemTitle()
    {
        return GetTextFieldValue("problemTitle");
    }

    public void EnterProblemCategory(string value)
    {
        EnterTextField("problemCategory", value);
    }

    public string? GetProblemCategory()
    {
        return GetTextFieldValue("problemCategory");
    }

    public void EnterProblemYear(string value)
    {
        EnterTextField("problemYear", value);
    }

    public string? GetProblemYear()
    {
        return GetTextFieldValue("problemYear");
    }

    public void EnterProblemNumber(string value)
    {
        EnterTextField("problemNumber", value);
    }

    public string? GetProblemNumber()
    {
        return GetTextFieldValue("problemNumber");
    }

    public void EnterProblemAnswer(string value)
    {
        EnterTextField("problemAnswer", value);
    }

    public string? GetProblemAnswer()
    {
        return GetTextFieldValue("problemAnswer");
    }

    public void EnterProblemTags(string value)
    {
        EnterTextField("problemTags", value);
    }

    public string? GetProblemTags()
    {
        return GetTextFieldValue("problemTags");
    }

    public void EnterAnswerOptions(string value)
    {
        EnterTextField("answerOptions", value);
    }

    public string? GetAnswerOptions()
    {
        return GetTextFieldValue("answerOptions");
    }

    public void EnterProblemText(string value)
    {
        EnterTextField("problemText", value, true);
    }

    public string? GetProblemText()
    {
        return GetTextFieldValue("problemText", true);
    }

    public void EnterSolutionText(string value)
    {
        EnterTextField("solutionText", value, true);
    }

    public string? GetSolutionText()
    {
        return GetTextFieldValue("solutionText", true);
    }

    public void ClickIsStaging()
    {
        IsNotLoading();
        var checkbox = _wait.Until(d => d.FindElement(By.CssSelector($"{_rootCssSelector} mat-checkbox[data-testid=\"isStaging\"] input[type=\"checkbox\"]")));
        if (checkbox != null)
        {
            checkbox.Click();
        }
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

    public void ClickSaveButton(bool confirm = true)
    {
        IsNotLoading();
        var button = _wait.Until(d => d.FindElement(By.CssSelector($"{_rootCssSelector} button[data-testid=\"btnSave\"]")));
        if (button != null && button.Enabled && button.Displayed)
        {
            button.Click();
            AlertHelper.HandleAlert(_wait, confirm); 
            IsNotLoading();
        }

    }

    public void ClickPreviewButton()
    {
        IsNotLoading();
        var button = _wait.Until(d => d.FindElement(By.CssSelector($"{_rootCssSelector} button[data-testid=\"btnPreview\"]")));
        if (button != null && button.Enabled && button.Displayed)
        {
            button.Click();
            IsNotLoading();
        }
    }

    public void ClickDeleteButton(bool confirm = true)
    {
        IsNotLoading();
        var button = _wait.Until(d => d.FindElement(By.CssSelector($"{_rootCssSelector} button[data-testid=\"btnDelete\"]")));
        if (button != null && button.Enabled && button.Displayed)
        {
            button.Click();
            AlertHelper.HandleAlert(_wait, confirm); 
            IsNotLoading();
        }
    }    

}
