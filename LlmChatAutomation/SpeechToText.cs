using OpenQA.Selenium;
using SeleniumUndetectedChromeDriver;
using System.Threading.Tasks;

namespace LlmChatAutomation
{

    //var speechToText = new SpeechToText();

    //await speechToText.InitAsync();

    //speechToText.Start();

    //Thread.Sleep(2000);

    //speechToText.StartRecording();

    //while (true)
    //{
    //    Thread.Sleep(3000);
    //    Console.WriteLine(speechToText.GetTranscription());
    //}

    //return;
//------------------------------


public class SpeechToText
    {
        private UndetectedChromeDriver _driver;

        public async Task InitAsync()
        {
            _driver = UndetectedChromeDriver.Create(driverExecutablePath: await new ChromeDriverInstaller().Auto());
            _driver.Manage().Window.Size = new System.Drawing.Size(800, 600);
        }

        public void Start()
        {
            _driver.GoToUrl("https://speechtyping.com/voice-typing/voice-to-text-polish");
        }

        public void StartRecording()
        {
            var cookiesButton = _driver.FindElement(By.Id("ez-accept-all"));
            cookiesButton.Click();

            Thread.Sleep(3000);

            var startButton = _driver.FindElement(By.ClassName("listen"));
            startButton.Click();
        }

        public string GetTranscription()
        {
            //textareaWrapper
            var speechDiv = _driver.FindElement(By.ClassName("ql-editor"));
            //var speechDiv = _driver.FindElement(By.Id("textareaWrapper"));
            var text = speechDiv.Text;
            return text;
        }
    }
}