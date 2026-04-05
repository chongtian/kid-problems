using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace KpUiTestxUnit.Pages;

public static class MatSelectHelper
{
    /// <summary>
    /// Selects an option from an Angular Material select component based on the visible label text.
    /// </summary>
    /// <param name="driver">an instance of IWebDriver</param>
    /// <param name="wait">an instance of WebDriverWait</param>
    /// <param name="select">the mat-select element</param>
    /// <param name="value">the visible label text of the option to select</param>
    /// <param name="rootCssSelector">(optiona) the root CSS selector for the select component</param>
    public static void SelectOptionByLabelValue(IWebDriver driver, WebDriverWait wait, IWebElement select, string value, string? rootCssSelector = null)
    {
        bool optionSelected = false;
        int MAX_ATTEMPTS = 3;

        select.Click();
        wait.Until(d => d.FindElements(By.CssSelector($"{rootCssSelector} div.cdk-overlay-pane mat-option")).Count > 0);
        var options = wait.Until(d => d.FindElements(By.CssSelector($"{rootCssSelector} div.cdk-overlay-pane mat-option")));
        if (options != null && options.Count > 0)
        {
            foreach (var option in options)
            {
                var label = option.FindElement(By.CssSelector("span"));
                if (label != null && label.Text.Contains(value))
                {
                    if (!option.Displayed || !option.Enabled)
                    {
                        ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView({block: 'center'});", option);
                        Console.WriteLine($"Option {value}: call javascript to scroll into view");
                    }

                    int attempt = 0;
                    while (attempt < MAX_ATTEMPTS)
                    {
                        try
                        {
                            option.Click();
                            wait.Until(d => d.FindElements(By.CssSelector($"{rootCssSelector} div.cdk-overlay-pane")).Count == 0);
                            optionSelected = true;
                            Console.WriteLine($"Option {value}: clicked, attempt {attempt + 1}");
                            break;
                        }
                        catch (WebDriverTimeoutException)
                        {
                            attempt++;
                        }
                    }
                    if(!optionSelected)
                    {
                        Console.WriteLine($"Option {value}: failed to click after {MAX_ATTEMPTS} attempts");
                    }
                    break;
                }
            }
        }
        

        Console.WriteLine($"Option {value} selected: {optionSelected}");
    }
}