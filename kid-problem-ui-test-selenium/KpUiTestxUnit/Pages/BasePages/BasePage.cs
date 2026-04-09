using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace KpUiTestxUnit.Pages;

public abstract class BasePage
{
    protected readonly IWebDriver _driver;
    protected readonly WebDriverWait _wait;
    protected readonly string _rootCssSelector;

    public BasePage(IWebDriver driver, string rootCssSelector = "")
    {
        _driver = driver;
        _wait = WebDriverUtility.GetWait(_driver, SetupFixture.TimeoutInSeconds);
        _rootCssSelector = rootCssSelector;
    }

    public virtual string GetPageTitle()
    {
        var pageTitle = _wait.Until(d => d.FindElement(By.CssSelector(".page-title")));
        if (pageTitle != null && pageTitle.Displayed)
        {
            return pageTitle.Text.Trim();
        }
        return "";
    }

    protected bool IsNotLoading(int timeoutInSeconds = 0)
    {
        var wait = timeoutInSeconds == 0 ? _wait : WebDriverUtility.GetWait(_driver, timeoutInSeconds);
        var globalOverlay = By.CssSelector(".global-overlay");
        var notLoading = wait.Until(d => d.FindElements(globalOverlay).Count == 0);

        // this block of codes ensures that snack bar is dismissed.
        try
        {
            var snackBar = _driver.FindElement(By.CssSelector("button[data-testid=\"btnClearMessages\"]"));
            if (snackBar is not null && snackBar.Enabled && snackBar.Displayed)
            {
                snackBar.Click();
            }
        }
        catch (NoSuchElementException)
        {
            // no action is required
        }

        notLoading = notLoading && wait.Until(d => d.FindElements(By.CssSelector("button[data-testid=\"btnClearMessages\"]")).Count == 0);
        return notLoading;
    }

    protected void EnterTextField(string testId, string value, bool isTextArea = false)
    {
        IsNotLoading();

        var selector = isTextArea ? $"{_rootCssSelector} textarea[data-testid=\"{testId}\"]" : $"{_rootCssSelector} input[data-testid=\"{testId}\"]";
        var textbox = _wait.Until(d => d.FindElement(By.CssSelector(selector)));
        if (textbox != null && textbox.Enabled && textbox.Displayed)
        {
            textbox.SendKeys(Keys.Control + "a");
            textbox.SendKeys(Keys.Backspace);
            textbox.SendKeys(value);
        }
    }

    protected string? GetTextFieldValue(string testId, bool isTextArea = false)
    {
        IsNotLoading();

        var selector = isTextArea ? $"{_rootCssSelector} textarea[data-testid=\"{testId}\"]" : $"{_rootCssSelector} input[data-testid=\"{testId}\"]";
        var textbox = _wait.Until(d => d.FindElement(By.CssSelector(selector)));
        if (textbox != null && textbox.Enabled && textbox.Displayed)
        {
            return textbox.GetAttribute("value");
        }
        return null;
    }

    protected string? GetViewOnlyTextFieldValue(string testId)
    {
        IsNotLoading();
        var textField = _wait.Until(d => d.FindElement(By.CssSelector($"{_rootCssSelector} div[data-testid=\"{testId}\"] span:nth-child(2)")));
        if (textField != null && textField.Enabled && textField.Displayed)
        {
            return textField.Text;
        }
        return null;
    }

    protected string TakeSnapshot(IWebElement element, string? name = null)
    {
        var screenshot = ((ITakesScreenshot)element).GetScreenshot();
        var fileName = Path.Combine(Directory.GetCurrentDirectory(), $"{name}{DateTime.Now:yyyyMMdd_HHmmss}.png");
        screenshot.SaveAsFile(fileName);
        return fileName;
    }

}