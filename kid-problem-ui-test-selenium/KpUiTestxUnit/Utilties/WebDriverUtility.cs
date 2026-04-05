using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace KpUiTestxUnit;

public class WebDriverUtility
{
    public static IWebDriver GetChromeDriver()
    {
        ChromeOptions options = new ChromeOptions();
        options.AddArgument("--headless");  // Run the browser in headless mode for CI environments
        // options.AddArgument("--timezone=America/Chicago"); // Test data are sensitive to timezone; This did not work.
        IWebDriver driver = new ChromeDriver(options);
        driver.Manage().Window.Size = new System.Drawing.Size(1920, 1080);
        try
        {
            ((ChromeDriver)driver).ExecuteCdpCommand("Emulation.setTimezoneOverride", new Dictionary<string, object> { { "timezoneId", "America/Chicago" } });
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
        return driver;
    }

    public static WebDriverWait GetWait(IWebDriver driver, int timeoutInSeconds = 10)
    {
        return new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
    }

    public static WebDriverWait GetShortWait(IWebDriver driver, int timeoutInMilliseconds = 250)
    {
        return new WebDriverWait(driver, TimeSpan.FromMilliseconds(timeoutInMilliseconds));
    }
}