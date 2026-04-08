using OpenQA.Selenium;

namespace KpUiTestxUnit.Pages;

public sealed class PaginatorComponentPage : BasePage
{

    public PaginatorComponentPage(IWebDriver driver, string? parentSelector = null) : base(driver, $"{parentSelector} mat-paginator".Trim())
    { }

    public void ClickFirstPageButton()
    {
        IsNotLoading();
        var button = _wait.Until(d => d.FindElement(By.CssSelector($"{_rootCssSelector} button[aria-label=\"First page\"]")));
        if (button != null && button.Enabled && button.Displayed)
        {
            button.Click();
            IsNotLoading();
        }
    }

    public void ClickPreviousPageButton()
    {
        IsNotLoading();
        var button = _wait.Until(d => d.FindElement(By.CssSelector($"{_rootCssSelector} button[aria-label=\"Previous page\"]")));
        if (button != null && button.Enabled && button.Displayed)
        {
            button.Click();
            IsNotLoading();
        }
    }

    public void ClickNextPageButton()
    {
        IsNotLoading();
        var button = _wait.Until(d => d.FindElement(By.CssSelector($"{_rootCssSelector} button[aria-label=\"Next page\"]")));
        if (button != null && button.Enabled && button.Displayed)
        {
            button.Click();
            IsNotLoading();
        }
    }

    public void ClickLastPageButton()
    {
        IsNotLoading();
        var button = _wait.Until(d => d.FindElement(By.CssSelector($"{_rootCssSelector} button[aria-label=\"Last page\"]")));
        if (button != null && button.Enabled && button.Displayed)
        {
            button.Click();
            IsNotLoading();
        }
    }

    public string? GetPaginationStatus()
    {
        IsNotLoading();
        var textField = _wait.Until(d => d.FindElement(By.CssSelector($"{_rootCssSelector} div[role=\"status\"]")));
        if (textField != null && textField.Displayed)
        {
            return textField.Text;
        }
        return null;
    }

    public void SelectItemsPerPage(string value)
    {
        IsNotLoading();
        var select = _wait.Until(d => d.FindElement(By.CssSelector($"{_rootCssSelector} mat-select")));
        if (select != null && select.Displayed && select.Enabled)
        {
            MatSelectHelper.SelectOptionByLabelValue(_driver, _wait, select, value);
            IsNotLoading();
        }
    }

}
