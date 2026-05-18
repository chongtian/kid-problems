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

        if (select != null && select.Enabled && select.Displayed)
        {
            select.Click();
        }
        else
        {
            throw new NotFoundException("Cannot find or click the given mat-select element.");
        }

        var panel = wait.Until(d =>
        {
            var panels = d.FindElements(By.CssSelector($"{rootCssSelector} .cdk-overlay-pane .mat-mdc-select-panel"));
            return panels.FirstOrDefault(p => p.Displayed);
        });

        wait.Until(d =>
        {
            var options = panel.FindElements(By.CssSelector("mat-option"));
            return options.Count > 0;
        });

        var option = wait.Until(d =>
        {
            var options = panel.FindElements(By.CssSelector("mat-option"));
            return options.FirstOrDefault(o =>
                o.Text.Trim().Equals(value)
            );
        }) ?? throw new NoSuchElementException($"Option '{value}' not found.");

        option.Click();

        wait.Until(d =>
        {
            var panels = d.FindElements(By.CssSelector($"{rootCssSelector} .cdk-overlay-pane .mat-mdc-select-panel"));
            return panels.Count == 0 || panels.All(p => !p.Displayed);
        });

    }
}