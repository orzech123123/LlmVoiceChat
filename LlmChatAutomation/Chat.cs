using OpenQA.Selenium;
using SeleniumUndetectedChromeDriver;

namespace LlmChatAutomation
{
    public class Chat
    {
        private UndetectedChromeDriver _driver;

        public async Task InitAsync()
        {
            _driver = UndetectedChromeDriver.Create(driverExecutablePath: await new ChromeDriverInstaller().Auto());
        }

        public void Start()
        {
            _driver.GoToUrl("https://chatgpt.com/");
        }

        public void SendPrompt(string prompt)
        {
            IWebElement searchBox = _driver.FindElement(By.Id("prompt-textarea"));
            searchBox.SendKeys($"{prompt} . In one sentence.");
            searchBox.Submit();
        }

        public string GetAnswer()
        {
            var articles = _driver.FindElements(By.TagName("article"));
            var lastArticle = articles.Skip(Math.Max(0, articles.Count() - 1)).First();

            return lastArticle.Text.Replace("ChatGPT said", "");
        }

        public void SkipLogoutPopup()
        {
            IJavaScriptExecutor js = _driver;
            js.ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");

            Thread.Sleep(1000);

            try
            {
                IWebElement stayLoggedInLink = _driver.FindElements(By.TagName("a"))
                    .FirstOrDefault(a => a.Text.Trim().Equals("Nie wylogowuj", StringComparison.OrdinalIgnoreCase));

                if (stayLoggedInLink != null)
                {
                    Console.WriteLine("Found 'Nie wylogowuj' link. Clicking it..."); //TODO ang ver - Stay logged in sth..
                    stayLoggedInLink.Click();
                    Thread.Sleep(2000);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error finding/clicking 'Nie wylogowuj': {e.Message}");
            }
        }
    }
}
