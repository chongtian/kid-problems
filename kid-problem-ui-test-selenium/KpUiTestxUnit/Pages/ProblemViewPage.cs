using OpenQA.Selenium;

namespace KpUiTestxUnit.Pages;

public sealed class ProblemViewPage : BasePage
{

    private readonly string rootCssSelector = "app-problem-detail";

    public ProblemViewPage(IWebDriver driver) : base(driver)
    { }

    public void GoTo(string problemTitle)
    {
        _driver.Navigate().GoToUrl($"{Constants.BASE_URL}/problem/view/{problemTitle}");
        IsNotLoading();
    }

    public string? GetProblemTitle()
    {
        IsNotLoading();
        var textfield = _wait.Until(d => d.FindElement(By.CssSelector($"{rootCssSelector} h4")));
        if (textfield != null && textfield.Enabled && textfield.Displayed)
        {
            return textfield.Text;
        }
        return null;
    }

    public string? GetProblemCategory()
    {
        return GetViewOnlyTextFieldValue("problemCategory");
    }

    public string? GetProblemYear()
    {
        return GetViewOnlyTextFieldValue("problemYear");
    }

    public string? GetProblemNumber()
    {
        return GetViewOnlyTextFieldValue("problemNumber");
    }

    public string? GetAnswerOptions()
    {
        return GetViewOnlyTextFieldValue("answerOptions");
    }

    public string? GetProblemTags()
    {
        return GetViewOnlyTextFieldValue("tags");
    }

    public string? GetIsStaging()
    {
        return GetViewOnlyTextFieldValue("isStaging");
    }

    public string? GetProblemText()
    {
        IsNotLoading();
        var textfield = _wait.Until(d => d.FindElement(By.CssSelector($"{rootCssSelector} div[data-testid=\"problemRichText\"]")));
        if (textfield != null && textfield.Enabled && textfield.Displayed)
        {
            // ProblemText can include svg and base64 encoded images. 
            // For simplicity purpose, here we return Text only
            return textfield.Text;
        }
        return null;
    }

    public bool? ClickProblemAnswer()
    {
        IsNotLoading();
        var textfield = _wait.Until(d => d.FindElement(By.CssSelector($"{rootCssSelector} details[data-testid=\"problemAnswer\"]")));
        if (textfield != null && textfield.Enabled && textfield.Displayed)
        {
            textfield.Click();
            // Console.WriteLine($"open attribute: {textfield.GetAttribute("open")}");
            if (Boolean.TryParse(textfield.GetAttribute("open"), out bool isOpened))
            {
                return isOpened;
            }
        }
        return null;
    }

    public string? GetProblemAnswer()
    {
        IsNotLoading();
        var textfield = _wait.Until(d => d.FindElement(By.CssSelector($"{rootCssSelector} details[data-testid=\"problemAnswer\"] span")));
        if (textfield != null && textfield.Enabled && textfield.Displayed)
        {
            return textfield.Text;
        }
        return null;
    }

    public bool? ClickSolutionText()
    {
        IsNotLoading();
        var textfield = _wait.Until(d => d.FindElement(By.CssSelector($"{rootCssSelector} details[data-testid=\"solutionText\"]")));
        if (textfield != null && textfield.Enabled && textfield.Displayed)
        {
            textfield.Click();
            // Console.WriteLine(textfield.GetAttribute("open"));
            if (Boolean.TryParse(textfield.GetAttribute("open"), out bool isOpened))
            {
                return isOpened;
            }
        }
        return null;
    }

    public string? GetSolutionText()
    {
        IsNotLoading();
        var textfield = _wait.Until(d => d.FindElement(By.CssSelector($"{rootCssSelector} details[data-testid=\"solutionText\"] div")));
        if (textfield != null && textfield.Enabled && textfield.Displayed)
        {
            // SolutionText can include svg and base64 encoded images. 
            // For simplicity purpose, here we return Text only
            return textfield.Text;
        }
        return null;
    }

    public void ClickPreviewButton()
    {
        IsNotLoading();
        var button = _wait.Until(d => d.FindElement(By.CssSelector($"{rootCssSelector} button[data-testid=\"btnPreview\"]")));
        if (button != null && button.Enabled && button.Displayed)
        {
            button.Click();
            IsNotLoading();
        }
    }

    public void ClickEditButton()
    {
        IsNotLoading();
        var button = _wait.Until(d => d.FindElement(By.CssSelector($"{rootCssSelector} a[data-testid=\"btnEdit\"]")));
        if (button != null && button.Enabled && button.Displayed)
        {
            button.Click();
            IsNotLoading();
        }
    }

    public string? GetPreviousProblem()
    {
        IsNotLoading();
        var link = _wait.Until(d => d.FindElement(By.CssSelector($"{rootCssSelector} a[data-testid=\"btnPrevious\"]")));
        if (link != null && link.Enabled && link.Displayed)
        {
            return link.GetAttribute("href");
        }
        return null;
    }

    public string? GetNextProblem()
    {
        IsNotLoading();
        var link = _wait.Until(d => d.FindElement(By.CssSelector($"{rootCssSelector} a[data-testid=\"btnNext\"]")));
        if (link != null && link.Enabled && link.Displayed)
        {
            return link.GetAttribute("href");
        }
        return null;
    }

}
