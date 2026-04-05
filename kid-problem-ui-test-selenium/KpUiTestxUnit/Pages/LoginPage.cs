using OpenQA.Selenium;

namespace KpUiTestxUnit.Pages
{
    public sealed class LoginPage : BasePage
    {
        private readonly By usernameField = By.Id("email");
        private readonly By passwordField = By.Id("password");
        private readonly By loginButton = By.TagName("button");

        public LoginPage(IWebDriver driver) : base(driver)
        { }

        public bool Login(string username, string password)
        {
            _driver.Navigate().GoToUrl(Constants.BASE_URL + "/login");

            usernameField.FindElement(_driver).SendKeys(username);
            passwordField.FindElement(_driver).SendKeys(password);
            loginButton.FindElement(_driver).Click();

            // due to the cold-start of AWS Lambda functions, the initial login needs a longer timeout time
            var longWait = WebDriverUtility.GetWait(_driver, 60);            
            try
            {
                longWait.Until(d => d.Url.Contains("/home"));
                IsNotLoading();
                return true;
            }
            catch (WebDriverTimeoutException)
            {
                return false;
            }
        }


    }
}
