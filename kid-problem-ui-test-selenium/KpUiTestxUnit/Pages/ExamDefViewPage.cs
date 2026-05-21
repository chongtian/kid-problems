using OpenQA.Selenium;

namespace KpUiTestxUnit.Pages;

public sealed class ExamDefViewPage : BasePage
{

    public ExamDefViewPage(IWebDriver driver) : base(driver, "app-exam-def-detail")
    { }

    public ExamDefViewPage GoTo(string id)
    {
        _driver.Navigate().GoToUrl($"{Constants.BASE_URL}/examdef/view/{id}");
        IsNotLoading();
        return this;
    }


    public string? GetExamTitle()
    {
        return GetViewOnlyTextFieldValue("examTitle");
    }

    public string? GetExamCategory()
    {
        return GetViewOnlyTextFieldValue("examCategory");
    }

    public string? GetExamYear()
    {
        return GetViewOnlyTextFieldValue("examYear");
    }

    public string? GetExamType()
    {
        return GetViewOnlyTextFieldValue("examType");
    }

    public string? GetMemo()
    {
        return GetViewOnlyTextFieldValue("memo");
    }

    public string? GetActiveFlag()
    {
        return GetViewOnlyTextFieldValue("examStatus");
    }

    public string[] GetProblemTitlesFromExamDetails()
    {
        IsNotLoading();
        string selector = $"{_rootCssSelector} span[data-testid=\"problemTitle\"]";
        _wait.Until(d => d.FindElements(By.CssSelector(selector)).Count > 0);
        var items = _wait.Until(d => d.FindElements(By.CssSelector(selector)));
        if (items.Count > 0)
        {
            var ret = new string[items.Count];
            for (int i = 0; i < items.Count; i++)
            {
                ret[i] = items[i].Text.Trim();
            }
            return ret;
        }

        return [];
    }

    public ExamDefViewPage ClickProblemTitleInExamDetails(string problemTitle)
    {
        IsNotLoading();
        string selector = $"{_rootCssSelector} span[data-testid=\"problemTitle\"]";
        _wait.Until(d => d.FindElements(By.CssSelector(selector)).Count > 0);
        var items = _wait.Until(d => d.FindElements(By.CssSelector(selector)));
        var item = items.FirstOrDefault(i => i.Text.Trim() == problemTitle);
        if (item != null && item.Displayed && item.Enabled)
        {
            item.Click();
            IsNotLoading();
        }

        return this;
    }


    public ExamDefViewPage ClickCreateAssignmentButton(bool confirm = true)
    {
        IsNotLoading();
        var button = _wait.Until(d => d.FindElement(By.CssSelector($"{_rootCssSelector} button[data-testid=\"btnCreateAssignment\"]")));
        if (button != null && button.Enabled && button.Displayed)
        {
            button.Click();
            AlertHelper.HandleAlert(_wait, confirm);
            IsNotLoading();
        }

        return this;
    }

    public ExamDefViewPage ClickEditButton()
    {
        IsNotLoading();
        var button = _wait.Until(d => d.FindElement(By.CssSelector($"{_rootCssSelector} button[data-testid=\"btnSwitchView\"]")));
        if (button != null && button.Enabled && button.Displayed)
        {
            button.Click();
            IsNotLoading();
        }

        return this;
    }

}
