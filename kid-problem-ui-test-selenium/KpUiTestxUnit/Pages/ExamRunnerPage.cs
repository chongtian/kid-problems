using System;
using KpUiTestxUnit.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.DevTools.V142.Fetch;

namespace KpUiTestxUnit.Pages;

public class ExamRunnerPage : BasePage
{
    public ExamRunnerPage(IWebDriver driver) : base(driver, "app-exam-runner")
    { }

    public ExamRunnerPage GoTo(string id)
    {
        _driver.Navigate().GoToUrl($"{Constants.BASE_URL}/examrun/run/{id}");
        IsNotLoading();
        return this;
    }

    public string? GetCurrentProblemTitle()
    {
        IsNotLoading();
        var textField = _wait.Until(d => d.FindElement(By.CssSelector($"{_rootCssSelector} mat-card-subtitle[data-testid=\"problemTitle\"]")));
        if (textField != null && textField.Displayed)
        {
            return textField.Text;
        }
        return null;
    }

    public ExamRunnerPage ClickAnswerButton(string answer)
    {
        IsNotLoading();
        var selector = $"{_rootCssSelector} button[data-testid=\"btnAnswers\"]";
        _wait.Until(d => d.FindElements(By.CssSelector(selector)).Count > 0);
        var buttons = _wait.Until(d => d.FindElements(By.CssSelector(selector)));
        for (int i = 0; i < buttons.Count; i++)
        {
            if ((buttons[i].Text ?? "").Contains(answer) && buttons[i].Displayed && buttons[i].Enabled)
            {
                buttons[i].Click();
                IsNotLoading();
                break;
            }
        }
        return this;
    }

    public ExamRunnerPage ClickNavigateButton(string number)
    {
        IsNotLoading();
        var selector = $"{_rootCssSelector} button[data-testid=\"navigateButtons\"]";
        _wait.Until(d => d.FindElements(By.CssSelector(selector)).Count > 0);
        var buttons = _wait.Until(d => d.FindElements(By.CssSelector(selector)));
        for (int i = 0; i < buttons.Count; i++)
        {
            if ((buttons[i].Text ?? "").Contains(number) && buttons[i].Displayed && buttons[i].Enabled)
            {
                buttons[i].Click();
                IsNotLoading();
                break;
            }
        }
        return this;
    }

    public ExamRunnerPage ClickDeleteAnswerButton(bool confirm = true)
    {
        IsNotLoading();
        var selector = $"{_rootCssSelector} button[data-testid=\"btnDeleteAnswer\"]";
        var button = _wait.Until(d => d.FindElement(By.CssSelector(selector)));
        if (button != null && button.Displayed && button.Enabled)
        {
            button.Click();
            AlertHelper.HandleAlert(_wait, confirm);
            IsNotLoading();
        }
        return this;
    }

    public ExamRunnerPage ClickGuessButton()
    {
        IsNotLoading();
        var selector = $"{_rootCssSelector} mat-slide-toggle[data-testid=\"btnIsGuess\"]";
        var button = _wait.Until(d => d.FindElement(By.CssSelector(selector)));
        if (button != null && button.Displayed && button.Enabled)
        {
            button.Click();
            IsNotLoading();
        }
        return this;
    }

    public ExamRunnerPage ClickSubmitButton()
    {
        IsNotLoading();
        var selector = $"{_rootCssSelector} button[data-testid=\"btnSubmit\"]";
        var button = _wait.Until(d => d.FindElement(By.CssSelector(selector)));
        if (button != null && button.Displayed && button.Enabled)
        {
            button.Click();
            IsNotLoading();
        }
        return this;
    }

    public ExamRunnerPage ClickCompleteButton(bool confirm = true)
    {
        IsNotLoading();
        var selector = $"{_rootCssSelector} button[data-testid=\"btnComplete\"]";
        var button = _wait.Until(d => d.FindElement(By.CssSelector(selector)));
        if (button != null && button.Displayed && button.Enabled)
        {
            button.Click();
            AlertHelper.HandleAlert(_wait, confirm);
            IsNotLoading();
        }
        return this;
    }

    public ExamRunnerPage ClickFinalCompleteButton(bool confirm = true)
    {
        IsNotLoading();
        // there are two buttons which have the same data-testid. This shall be corrected in the future.
        var selector = $"{_rootCssSelector} mat-expansion-panel button[data-testid=\"btnComplete\"]";
        var button = _wait.Until(d => d.FindElement(By.CssSelector(selector)));
        if (button != null && button.Displayed && button.Enabled)
        {
            button.Click();
            IsNotLoading();
        }
        return this;
    }

    public ExamRunnerPage ClickNotCompleteButton()
    {
        IsNotLoading();
        var selector = $"{_rootCssSelector} button[data-testid=\"btnNotComplete\"]";
        var button = _wait.Until(d => d.FindElement(By.CssSelector(selector)));
        if (button != null && button.Displayed && button.Enabled)
        {
            button.Click();
            IsNotLoading();
        }
        return this;
    }

    public ExamRunnerPage ClickExpandSummaryPanel()
    {
        IsNotLoading();
        var selector = $"{_rootCssSelector} mat-expansion-panel-header";
        var button = _wait.Until(d => d.FindElement(By.CssSelector(selector)));
        if (button != null && button.Displayed && button.Enabled)
        {
            button.Click();
            IsNotLoading();
        }
        return this;
    }

    public ExamRunDetail[]? GetExamRunDetail()
    {
        IsNotLoading();
        var selector = $"{_rootCssSelector} table[data-testid=\"reviewAnswers\"]";
        var table = _wait.Until(d => d.FindElement(By.CssSelector(selector)));
        if (table != null && table.Displayed)
        {
            var rows = table.FindElements(By.CssSelector("tbody tr"));
            var results = new ExamRunDetail[rows.Count];
            for (int i = 0; i < rows.Count; i++)
            {
                var record = new ExamRunDetail
                {
                    UserAnswer = rows[i].FindElement(By.CssSelector("td[data-testid=\"answer\"]")).Text.Trim(),
                    Guess = rows[i].FindElement(By.CssSelector("td[data-testid=\"guess\"]")).Text.Trim(),
                    Duration = rows[i].FindElement(By.CssSelector("td[data-testid=\"duration\"]")).Text.Trim(),
                    ProblemTitle = rows[i].FindElement(By.CssSelector("td[data-testid=\"problem\"]")).Text.Trim()
                };
                results[i] = record;
            }
            return results;
        }
        return [];
    }

    public bool IsReviewButtonsHidden()
    {
        IsNotLoading();
        var selector = $"{_rootCssSelector} button[data-testid=\"btnNotComplete\"]";
        bool isHidden = _wait.Until(d => d.FindElements(By.CssSelector(selector)).Count == 0);
        selector = $"{_rootCssSelector} mat-expansion-panel button[data-testid=\"btnComplete\"]";
        isHidden = isHidden && _wait.Until(d => d.FindElements(By.CssSelector(selector)).Count == 0);
        return isHidden;
    }

    public bool IsReviewButtonsShown()
    {
        IsNotLoading();
        var selector = $"{_rootCssSelector} button[data-testid=\"btnNotComplete\"]";
        bool isShown = _wait.Until(d => d.FindElements(By.CssSelector(selector)).Count > 0);
        selector = $"{_rootCssSelector} mat-expansion-panel button[data-testid=\"btnComplete\"]";
        isShown = isShown && _wait.Until(d => d.FindElements(By.CssSelector(selector)).Count > 0);
        return isShown;
    }    

    public string? GetNavigateButtonColor(string number)
    {
        IsNotLoading();
        var selector = $"{_rootCssSelector} button[data-testid=\"navigateButtons\"]";
        _wait.Until(d => d.FindElements(By.CssSelector(selector)).Count > 0);
        var buttons = _wait.Until(d => d.FindElements(By.CssSelector(selector)));
        for (int i = 0; i < buttons.Count; i++)
        {
            if ((buttons[i].Text ?? "").Contains(number) && buttons[i].Displayed && buttons[i].Enabled)
            {
                return buttons[i].GetCssValue("background-color");
            }
        }

        return null;
    }

    public string? GetAnswerButtonColor(string number)
    {
        IsNotLoading();
        var selector = $"{_rootCssSelector} button[data-testid=\"btnAnswers\"]";
        _wait.Until(d => d.FindElements(By.CssSelector(selector)).Count > 0);
        var buttons = _wait.Until(d => d.FindElements(By.CssSelector(selector)));
        for (int i = 0; i < buttons.Count; i++)
        {
            if ((buttons[i].Text ?? "").Contains(number) && buttons[i].Displayed && buttons[i].Enabled)
            {
                return buttons[i].GetCssValue("background-color");
            }
        }
        
        return null;
    }    

}
