using KpUiTestxUnit.Models;
using OpenQA.Selenium;

namespace KpUiTestxUnit.Pages;

public sealed class ExamRunViewPage : BasePage
{

    public ExamRunViewPage(IWebDriver driver) : base(driver, "app-exam-run-detail")
    { }

    public void GoTo(string id)
    {
        _driver.Navigate().GoToUrl($"{Constants.BASE_URL}/examrun/view/{id}");
        IsNotLoading();
    }

    public string? GetExamTitle()
    {
        return GetViewOnlyTextFieldValue("examTitle");
    }

    public string? GetExamCategory()
    {
        return GetViewOnlyTextFieldValue("examCategory");
    }

    public string? GetAnsweredBy()
    {
        return GetViewOnlyTextFieldValue("answerBy");
    }

    public string? GetStartTime()
    {
        return GetViewOnlyTextFieldValue("startTime");
    }

    public string? GetCompleteTime()
    {
        return GetViewOnlyTextFieldValue("completeTime");
    }

    public string? GetTotalDuration()
    {
        return GetViewOnlyTextFieldValue("totalDuration");
    }

    public string? GetTotalCount()
    {
        return GetViewOnlyTextFieldValue("totalCount");
    }

    public string? GetCorrectCount()
    {
       return GetViewOnlyTextFieldValue("correctCount");
    }

    public string? GetGuessCount()
    {
        return GetViewOnlyTextFieldValue("guessCount");
    }

    public string? GetGuessCorrectCount()
    {
        return GetViewOnlyTextFieldValue("guessCorrectCount");
    }

    public ExamRunDetail[] GetExamRunDetails()
    {
        IsNotLoading();
        var selector = By.CssSelector($"{_rootCssSelector} tbody tr");
        var hasResult = _wait.Until(d => d.FindElements(selector).Count > 0);
        if (hasResult)
        {
            var rows = _wait.Until(d => d.FindElements(selector));
            var ret = new ExamRunDetail[rows.Count];
            for (int i = 0; i < rows.Count; i++)
            {
                var problemTitle = rows[i].FindElement(By.CssSelector("td[data-testid=\"problemTitle\"]")).Text.Trim();
                var userAnswer = rows[i].FindElement(By.CssSelector("td[data-testid=\"userAnswer\"]")).Text.Trim();              
                var isCorrect = rows[i].FindElement(By.CssSelector("td[data-testid=\"isCorrect\"]")).Text.Trim();
                var isGuess = rows[i].FindElement(By.CssSelector("td[data-testid=\"isGuess\"]")).Text.Trim();
                var duration = rows[i].FindElement(By.CssSelector("td[data-testid=\"duration\"]")).Text.Trim();

                ret[i] = new ExamRunDetail
                {
                    ProblemTitle = problemTitle,
                    UserAnswer = userAnswer,
                    Correct = isCorrect,
                    Guess = isGuess,
                    Duration = duration
                };
            }
            return ret;
        }

        return [];
    }
    public void ClickProblemTitleInExamRunDetails(int index)
    {
        IsNotLoading();
        var selector = By.CssSelector($"{_rootCssSelector} tbody tr");
        var hasResult = _wait.Until(d => d.FindElements(selector).Count > 0);
        if (hasResult)
        {
            var rows = _wait.Until(d => d.FindElements(selector));
            var row = rows.ElementAtOrDefault(index);
            if (row != null)
            {
                var link = row.FindElement(By.CssSelector("td[data-testid=\"problemTitle\"] a"));
                if (link != null && link.Enabled && link.Displayed)
                {
                    link.Click();
                    IsNotLoading();
                }
            }
        }
    }

    public void ClickGoToAssignment()
    {
        var link = _wait.Until(d => d.FindElement(By.CssSelector($"{_rootCssSelector} span[data-testid=\"linkToAssignment\"] a")));
        if (link != null && link.Enabled && link.Displayed)
        {
            link.Click();
            IsNotLoading();
        }
    }

}
