using OpenQA.Selenium;

namespace KpUiTestxUnit.Pages;

public sealed class ExamDefEditPage : BasePage
{

    public QueryProblemsPage QueryPage { get; private set; }

    public ExamDefEditPage(IWebDriver driver) : base(driver, "app-exam-def-detail")
    {
        QueryPage = new QueryProblemsPage(driver);
    }

    public void GoTo(string? id = null)
    {
        if (string.IsNullOrEmpty(id))
        {
            _driver.Navigate().GoToUrl($"{Constants.BASE_URL}/examdef/create");
        }
        else
        {
            _driver.Navigate().GoToUrl($"{Constants.BASE_URL}/examdef/edit/{id}");
        }

        IsNotLoading();
    }

    public void SelectExamCategory(string value)
    {
        IsNotLoading();
        var select = _wait.Until(d => d.FindElement(By.CssSelector($"{_rootCssSelector} mat-select[data-testid=\"examCategory\"]")));
        if (select != null && select.Displayed && select.Enabled)
        {
            MatSelectHelper.SelectOptionByLabelValue(_driver, _wait, select, value, _rootCssSelector);
        }
    }

    public string GetExamCategory()
    {
        IsNotLoading();
        var selector = _wait.Until(d => d.FindElement(By.CssSelector($"{_rootCssSelector} mat-select[data-testid=\"examCategory\"]")));
        return selector.Text;
    }


    public void EnterExamTitle(string value)
    {
        EnterTextField("examTitle", value);
    }

    public string? GetExamTitle()
    {
        return GetTextFieldValue("examTitle");
    }

    public void EnterExamYear(string value)
    {
        EnterTextField("examYear", value);
    }

    public string? GetExamYear()
    {
        return GetTextFieldValue("examYear");
    }

    public void EnterMemo(string value)
    {
        EnterTextField("memo", value);
    }

    public string? GetMemo()
    {
        return GetTextFieldValue("memo");
    }

    public void ClickExamStatus()
    {
        IsNotLoading();
        var checkbox = _wait.Until(d => d.FindElement(By.CssSelector($"{_rootCssSelector} mat-checkbox[data-testid=\"examStatus\"] input[type=\"checkbox\"]")));
        if (checkbox != null)
        {
            checkbox.Click();
        }
    }

    public bool? GetExamStatus()
    {
        IsNotLoading();
        var checkbox = _wait.Until(d => d.FindElement(By.CssSelector($"{_rootCssSelector} mat-checkbox[data-testid=\"examStatus\"] input[type=\"checkbox\"]")));
        if (checkbox != null)
        {
            return checkbox.Selected;
        }
        return null;
    }

    public void SelectExamType(string value)
    {
        IsNotLoading();
        var select = _wait.Until(d => d.FindElement(By.CssSelector($"{_rootCssSelector} mat-select[data-testid=\"examType\"]")));
        if (select != null && select.Displayed && select.Enabled)
        {
            MatSelectHelper.SelectOptionByLabelValue(_driver, _wait, select, value, _rootCssSelector);
        }
    }

    public string GetExamType()
    {
        IsNotLoading();
        var selector = _wait.Until(d => d.FindElement(By.CssSelector($"{_rootCssSelector} mat-select[data-testid=\"examType\"]")));
        return selector.Text;
    }

    public void ClickAddProblemButton()
    {
        IsNotLoading();
        var button = _wait.Until(d => d.FindElement(By.CssSelector($"{_rootCssSelector} button[data-testid=\"btnAddProblems\"]")));
        if (button != null && button.Enabled && button.Displayed)
        {
            button.Click();
            IsNotLoading();
        }
    }

    public string[] GetExamDetails()
    {
        IsNotLoading();
        _wait.Until(d => d.FindElements(By.CssSelector($"{_rootCssSelector} span[data-testid=\"problemTitle\"]")).Count > 0);
        var items = _wait.Until(d => d.FindElements(By.CssSelector($"{_rootCssSelector} span[data-testid=\"problemTitle\"]")));
        if (items.Count > 0)
        {
            var ret = new string[items.Count];
            for (int i = 0; i < items.Count; i++)
            {
                ret[i] = items[i].Text.Trim();
            }
            return ret;
        }
        else
        {
            return [];
        }
    }

    public void ClickDeleteButtonOnExamDetails(int index)
    {
        IsNotLoading();
        var buttons = _wait.Until(d => d.FindElements(By.CssSelector($"{_rootCssSelector} div[data-testid=\"problem\"] button[data-testid=\"btnDelete\"]")));
        if (buttons.Count > 0)
        {
            var button = buttons.ElementAt(index);
            if (button != null && button.Enabled && button.Displayed)
            {
                button.Click();
            }
        }
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

    public void ClickBackToViewButton()
    {
        IsNotLoading();
        var button = _wait.Until(d => d.FindElement(By.CssSelector($"{_rootCssSelector} button[data-testid=\"btnSwitchView\"]")));
        if (button != null && button.Enabled && button.Displayed)
        {
            button.Click();
            IsNotLoading();
        }
    }

}
