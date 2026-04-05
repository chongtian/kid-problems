using System.Runtime.CompilerServices;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using RestSharp;

namespace KpUiTestxUnit
{
    public abstract class TestBase : IDisposable
    {
        protected readonly IWebDriver _driver;
        protected readonly WebDriverWait _wait;
        protected readonly SetupFixture _fixture;
        protected readonly RestClient _client;

        public TestBase(SetupFixture fixture, bool isAdminUser = true)
        {
            _fixture = fixture;
            _driver = fixture.GetDriverAndInjectSession(isAdminUser);
            _client = fixture.GetRestClient();
            _wait = WebDriverUtility.GetWait(_driver, SetupFixture.TimeoutInSeconds);
        }

        protected void RunTest(Action test, [CallerMemberName] string testName = "")
        {
            try
            {
                test();
            }
            catch
            {
                TakeScreenshot(testName);
                throw;
            }
        }

        protected async Task RunTestAsync(Func<Task> test, [CallerMemberName] string testName = "")
        {
            try
            {
                await test();
            }
            catch
            {
                TakeScreenshot(testName);
                throw;
            }
        }        

        private void TakeScreenshot(string testName)
        {
            var screenshot = ((ITakesScreenshot)_driver).GetScreenshot();
            var folder = Path.Combine(Directory.GetCurrentDirectory(), "Screenshots");
            Directory.CreateDirectory(folder);
            var fileName = $"{testName}_{DateTime.Now:yyyyMMdd_HHmmss}.png";
            screenshot.SaveAsFile(Path.Combine(folder, fileName));
        }

        protected async Task<T?> GetCall<T>(string url)
        {
            var request = new RestRequest(url);
            var response = await _client.ExecuteAsync<T>(request);
            if (response.IsSuccessful)
            {
                return response.Data;
            }
            return default;
        }

        protected async Task<string?> DeleteCall(string url)
        {
            var request = new RestRequest(url, Method.Delete);
            var response = await _client.ExecuteAsync(request);
            if (response.IsSuccessful)
            {
                return response.Content;
            }
            return null;
        }

        protected async Task<string?> PostCall(string url, object payload)
        {
            var request = new RestRequest(url, Method.Post);
            if (payload is string json)
            {
                request.AddStringBody(json, DataFormat.Json);
            }
            else
            {
                request.AddJsonBody(payload);
            }
            var response = await _client.ExecuteAsync(request);
            if (response.IsSuccessful)
            {
                return response.Content;
            }
            return null;
        }

        protected async Task<string?> PutCall(string url, object payload)
        {
            var request = new RestRequest(url, Method.Put); 
            if (payload is string json)
            {
                request.AddStringBody(json, DataFormat.Json);
            }
            else
            {
                request.AddJsonBody(payload);
            }
            var response = await _client.ExecuteAsync(request); 
            if (response.IsSuccessful)
            {
                return response.Content;
            }
            return $"{response.StatusCode}:{response.Content}";
        }

        public void Dispose()
        {
            _driver.Quit();
            _driver.Dispose();
        }

    }
}
