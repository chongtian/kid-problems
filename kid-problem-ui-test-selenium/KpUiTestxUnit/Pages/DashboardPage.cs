using OpenQA.Selenium;

namespace KpUiTestxUnit.Pages;

public class DashboardPage : BasePage
{
    public DashboardPage(IWebDriver driver) : base(driver, "app-home") { }

    public void GoTo()
    {
        _driver.Navigate().GoToUrl($"{Constants.HOME_URL}");
    }

    public string[] GetDashboardItemTitles()
    {
        IsNotLoading();
        string selector = $"{_rootCssSelector} .page-title";
        _wait.Until(d => d.FindElements(By.CssSelector(selector)).Count > 0);
        var elements = _driver.FindElements(By.CssSelector(selector));
        return [.. elements.Select(e => e.Text)];
    }

    public string[] GetChildExamStatistics()
    {
        IsNotLoading();
        string selector = $"{_rootCssSelector} div[data-testid=\"viewExamSummary\"]";
        _wait.Until(d => d.FindElements(By.CssSelector(selector)).Count > 0);
        var elements = _driver.FindElements(By.CssSelector(selector));
        return [.. elements.Select(e => e.Text)];
    }
}
