using System.Text.RegularExpressions;
using KpUiTestxUnit;
using KpUiTestxUnit.Pages;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using RestSharp;

[assembly: AssemblyFixture(typeof(SetupFixture))]

namespace KpUiTestxUnit
{
    public class SetupFixture : IDisposable
    {

        private readonly Dictionary<string, IReadOnlyCollection<Cookie>> _authCookieStore = new();
        private readonly Dictionary<string, string> _localStorageStore = new();
        private readonly string AdminUserKey = "admin";
        private readonly string ChildUserKey = "child";

        private readonly string EnvVarPrefix = "KPUITEST_";
        private readonly string AdminUsernameEnvVarName = "ADMIN_USERNAME";
        private readonly string AdminPasswordEnvVarName = "ADMIN_PASSWORD";
        private readonly string ChildUsernameEnvVarName = "CHILD_USERNAME";
        private readonly string ChildPasswordEnvVarName = "CHILD_PASSWORD";

        public static string AdminUsername { get; private set; } = "";
        public static string AdminPassword { get; private set; } = "";
        public static string ChildUsername { get; private set; } = "";
        public static string ChildPassword { get; private set; } = "";
        public static string BaseUrl { get; private set; } = "";
        public static string BaseApiUrl { get; private set; } = "";
        public static int TimeoutInSeconds { get; private set; } = 10;
        private static RestClient? _restClient = null;

        public SetupFixture()
        {
            ReadConfigurations();
            InitialLogin(true, 3);
            InitialLogin(false, 3);
            InitializeRestClient();
        }

        public IWebDriver GetDriverAndInjectSession(bool isAdminUser = true)
        {
            IWebDriver driver = WebDriverUtility.GetChromeDriver();

            // 1. Navigate to the domain first
            driver.Navigate().GoToUrl(Constants.BASE_URL);

            string userKey = isAdminUser ? AdminUserKey : ChildUserKey;

            // 2. Inject Cookies
            foreach (var cookie in _authCookieStore[userKey])
            {
                driver.Manage().Cookies.AddCookie(cookie);
            }

            // 3. Inject Local Storage (via JS)
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            js.ExecuteScript($"var data = {_localStorageStore[userKey]}; for(var key in data) {{ localStorage.setItem(key, data[key]); }}");

            // 4. Refresh to reflect the logged-in state
            driver.Navigate().GoToUrl(Constants.HOME_URL);
            driver.Navigate().Refresh();

            return driver;
        }

        public RestClient GetRestClient()
        {
            return _restClient ?? throw new NullReferenceException("Rest Client is not initialized.");
        }

        private void InitializeRestClient()
        {
            // get Bearer Token
            string token = "";
            var s = _localStorageStore[AdminUserKey];
            string pattern = @"accessToken"":""(.*?)""";
            RegexOptions options = RegexOptions.Multiline;
            var m = Regex.Match(s, pattern, options);
            if (m.Success)
            {
                token = m.Groups[1].Value;
            }

            if (!string.IsNullOrEmpty(token))
            {
                var restClientOptions = new RestClientOptions(BaseApiUrl);
                var client = new RestClient(restClientOptions);
                client.AddDefaultHeader("Authorization", $"Bearer {token}");
                client.AddDefaultHeader("Accept", "application/json");
                _restClient = client;
                Console.WriteLine("Rest Client has been initialized.");
            }
        }

        public void Dispose()
        {
            // Runs ONCE after all tests 
        }

        private void ReadConfigurations()
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;

            var config = new ConfigurationBuilder()
                            .SetBasePath(basePath)
                            .AddJsonFile("testsettings.json", optional: false, reloadOnChange: false)
                            .AddJsonFile("testsettings.local.json", optional: true, reloadOnChange: false)
                            .AddEnvironmentVariables(prefix: EnvVarPrefix)
                            .Build();

            AdminUsername = config["Credentials:AdminUsername"] ?? "";
            AdminPassword = config["Credentials:AdminPassword"] ?? "";
            ChildUsername = config["Credentials:ChildUsername"] ?? "";
            ChildPassword = config["Credentials:ChildPassword"] ?? "";
            BaseUrl = config["BaseUrl"] ?? "";
            BaseApiUrl = config["BaseApiUrl"] ?? "";
            if (int.TryParse(config["TimeoutInSeconds"] ?? "10", out int t) && t > 0)
            {
                TimeoutInSeconds = t;
            }

            // If Username or Password is blank, try to get then from Environment Variables
            if (string.IsNullOrEmpty(AdminUsername))
            {
                AdminUsername = config[AdminUsernameEnvVarName] ?? "";
            }
            if (string.IsNullOrEmpty(AdminPassword))
            {
                AdminPassword = config[AdminPasswordEnvVarName] ?? "";
            }
            if (string.IsNullOrEmpty(ChildUsername))
            {
                ChildUsername = config[ChildUsernameEnvVarName] ?? "";
            }
            if (string.IsNullOrEmpty(ChildPassword))
            {
                ChildPassword = config[ChildPasswordEnvVarName] ?? "";
            }

            if (string.IsNullOrWhiteSpace(BaseUrl))
            {
                Assert.Fail("BaseUrl not provided. Set the item in testsettings.json.");
            }

            if (string.IsNullOrWhiteSpace(BaseApiUrl))
            {
                Assert.Fail("BaseApiUrl not provided. Set the item in testsettings.json.");
            }

            if (string.IsNullOrWhiteSpace(AdminUsername) || string.IsNullOrWhiteSpace(AdminPassword)
                || string.IsNullOrWhiteSpace(ChildUsername) || string.IsNullOrWhiteSpace(ChildPassword))
            {
                Assert.Fail(
                    "Credentials not provided. Set them in testsettings.json or via env vars. ");
            }
        }


        private void InitialLogin(bool isAdminUser, int maxAttempts)
        {
            string username = isAdminUser ? AdminUsername : ChildUsername;
            string password = isAdminUser ? AdminPassword : ChildPassword;
            string storeKey = isAdminUser ? AdminUserKey : ChildUserKey;
            bool isSuccessful = false;
            int cnt = 0;

            while (cnt < maxAttempts && !isSuccessful)
            {
                IWebDriver driver = WebDriverUtility.GetChromeDriver();

                var loginPage = new LoginPage(driver);
                if (loginPage.Login(username, password))
                {
                    var cookies = driver.Manage().Cookies.AllCookies;
                    _authCookieStore.Add(storeKey, cookies);

                    IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                    if (js != null)
                    {
                        var localData = js.ExecuteScript("return JSON.stringify(localStorage);");
                        if (localData != null)
                        {
                            _localStorageStore.Add(storeKey, localData.ToString() ?? "");
                        }
                    }
                    isSuccessful = true;
                }

                driver.Quit();
                driver.Dispose();

                cnt++;
            }

            if (!isSuccessful)
            {
                Assert.Fail($"Failed to login {storeKey} user after {maxAttempts} attempts.");
            }
            else
            {
                Console.WriteLine($"Successfully log in {storeKey} user in {cnt} attempt(s).");
            }

        }

    }
}