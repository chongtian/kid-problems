using OpenQA.Selenium;

namespace KpUiTestxUnit.Pages;

public sealed class AssignmentViewPage : BasePage
{
    public AssignmentViewPage(IWebDriver driver) : base(driver, "app-assignment-detail")
    { }

    public AssignmentViewPage GoTo(string id)
    {
        _driver.Navigate().GoToUrl($"{Constants.BASE_URL}/assignment/view/{id}");
        IsNotLoading();
        return this;
    }

    public string? GetExamCategory()
    {
        return GetMatListItemValue("examCategory");
    }

    public string? GetExamTitle()
    {
        return GetMatListItemValue("examTitle");
    }

    public string? GetCreateTime()
    {
        return GetMatListItemValue("createTime");
    }

    public string? GetMemo()
    {
        return GetMatListItemValue("memo");
    }

    public string? GetCompleted()
    {
        return GetMatListItemValue("isComplete");
    }

    public Tuple<string, string>[] GetExamRuns()
    {
        IsNotLoading();
        string selector = $"{_rootCssSelector} span[data-testid=\"examRuns\"] a";
        _wait.Until(d => d.FindElements(By.CssSelector(selector)).Count > 0);
        var items = _wait.Until(d => d.FindElements(By.CssSelector(selector)));
        if (items.Count > 0)
        {
            var ret = new Tuple<string, string>[items.Count];
            for (int i = 0; i < items.Count; i++)
            {
                ret[i] = new Tuple<string, string>(items[i].Text, items[i].GetAttribute("href") ?? "");
            }
            return ret;
        }

        return [];
    }

    private string? GetMatListItemValue(string testid)
    {
        IsNotLoading();
        var listItem = _wait.Until(d => d.FindElement(By.CssSelector($"{_rootCssSelector} mat-list-item span[data-testid=\"{testid}\"]")));
        if (listItem != null && listItem.Displayed)
        {
            return listItem.Text;
        }
        return null;
    }

}
