using NAudio.Lame;
using NAudio.Wave;
using OpenQA.Selenium;
using SeleniumUndetectedChromeDriver;
using System.Diagnostics;
using System.Globalization;
using System.Speech.Synthesis;

class Program
{
    static bool _pl = false;

    static async Task Main()
    {
        using (var driver = UndetectedChromeDriver.Create(driverExecutablePath: await new ChromeDriverInstaller().Auto()))
        {
            driver.GoToUrl("https://chatgpt.com/");

            Thread.Sleep(2000); 

            while (true)
            {
                IJavaScriptExecutor js = driver;
                js.ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");

                Thread.Sleep(1000);

                try
                {
                    IWebElement stayLoggedInLink = driver.FindElements(By.TagName("a"))
                        .FirstOrDefault(a => a.Text.Trim().Equals("Nie wylogowuj", StringComparison.OrdinalIgnoreCase));

                    if (stayLoggedInLink != null)
                    {
                        Console.WriteLine("Found 'Nie wylogowuj' link. Clicking it...");
                        stayLoggedInLink.Click();
                        Thread.Sleep(2000);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error finding/clicking 'Nie wylogowuj': {e.Message}");
                }

                IWebElement searchBox = driver.FindElement(By.Id("prompt-textarea"));

                Random rnd = new Random();
                //string prompt = questions[rnd.Next(questions.Count)];

                RecordVoice();

                var prompt = TranscribeVoice();

                searchBox.SendKeys($"{prompt} . In one sentence.");

                searchBox.Submit();

                Thread.Sleep(3000);

                var articles = driver.FindElements(By.TagName("article"));
                var lastArticle = articles.Skip(Math.Max(0, articles.Count() - 1)).First();

                var textToRead = lastArticle.Text.Replace("ChatGPT said", "");
                ReadText(textToRead);
            }
        }

        // NOTE: very inaccurate transcription from audio source:
        //
        //using (SpeechRecognitionEngine recognizer = new SpeechRecognitionEngine())
        //{
        //    // Set up the grammar to recognize any words
        //    recognizer.LoadGrammar(new DictationGrammar());

        //    // Configure input to be from the default microphone
        //    recognizer.SetInputToDefaultAudioDevice();

        //    // Attach an event handler for recognized speech
        //    recognizer.SpeechRecognized += (sender, e) =>
        //    {
        //        Console.WriteLine("You said: " + e.Result.Text);
        //    };

        //    Console.WriteLine("Speak into your microphone...");

        //    // Start recognition (listens synchronously)
        //    recognizer.RecognizeAsync(RecognizeMode.Multiple);

        //    // Keep the console running
        //    Console.ReadLine();
        //}
    }

    private static void ReadText(string text)
    {
        var synthesizer = new SpeechSynthesizer();
        synthesizer.SetOutputToDefaultAudioDevice();

        //synthesizer.SelectVoice("ScanSoft Virginie_Dri40_16kHz");

        // or this
        if(_pl)
        {
            synthesizer.SelectVoiceByHints(VoiceGender.Male, VoiceAge.Adult, 0, CultureInfo.GetCultureInfo("pl-PL"));
        }
        else
        {
            synthesizer.SelectVoiceByHints(VoiceGender.Male, VoiceAge.Adult, 0, CultureInfo.GetCultureInfo("en-US"));
        }

        synthesizer.Speak(text);
    }

    private static string TranscribeVoice()
    {
        var language = _pl ? "Polish" : "English";

        string exePath = @"C:\\Projects\\Faster-Whisper-XXL_r192.3.4_windows\\Faster-Whisper-XXL\\faster-whisper-xxl.exe";
        string arguments = $@".\recorded.mp3 --language {language} --model medium --output_dir source";

        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = exePath,
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (Process process = new Process { StartInfo = psi })
        {
            process.OutputDataReceived += (sender, e) => Console.WriteLine(e.Data);
            process.ErrorDataReceived += (sender, e) => Console.WriteLine("Error: " + e.Data);

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            process.WaitForExit();
        }

        string filePath = "recorded.srt"; 

        if (File.Exists(filePath))
        {
            string[] lines = File.ReadAllLines(filePath);

            return lines[2];
        }

        return null;
    }

    static void RecordVoice()
    {
        Console.WriteLine("Recording started..");
        //Console.ReadLine();
        StartRecording();

        Console.WriteLine("Recording... Press Enter to stop.");
        Console.ReadLine();
        StopRecording();

        Console.WriteLine("Saving as MP3...");
        ConvertWavToMp3(outputWavFile, outputMp3File);

        Console.WriteLine("Recording saved as recorded.mp3");
    }

    static void StartRecording()
    {
        waveSource = new WaveInEvent
        {
            WaveFormat = new WaveFormat(44100, 1) 
        };

        waveSource.DataAvailable += (s, e) =>
        {
            waveFile.Write(e.Buffer, 0, e.BytesRecorded);
        };

        waveFile = new WaveFileWriter(outputWavFile, waveSource.WaveFormat);
        waveSource.StartRecording();
    }

    static void StopRecording()
    {
        waveSource.StopRecording();
        waveSource.Dispose();
        waveFile.Dispose();
    }

    static void ConvertWavToMp3(string wavFilePath, string mp3FilePath)
    {
        using (var reader = new AudioFileReader(wavFilePath))
        using (var writer = new LameMP3FileWriter(mp3FilePath, reader.WaveFormat, LAMEPreset.ABR_128))
        {
            reader.CopyTo(writer);
        }

        File.Delete(wavFilePath);
    }

    static WaveInEvent waveSource;
    static WaveFileWriter waveFile;
    static string outputWavFile = "recorded.wav";
    static string outputMp3File = "recorded.mp3";

    static List<string> questions = new List<string>
        {
            "What is your purpose in life?",
            "If you could have one superpower, what would it be?",
            "What is the meaning of true happiness?",
            "If you could visit any place in the world, where would you go?",
            "What is your biggest fear?",
            "If you had a time machine, which era would you visit?",
            "What inspires you the most?",
            "What is your favorite childhood memory?",
            "If you could meet any historical figure, who would it be?",
            "What is your dream job?",
            "If you won the lottery, what’s the first thing you would do?",
            "What is the best advice you have ever received?",
            "If you could change one thing about the world, what would it be?",
            "What is the most challenging thing you’ve ever done?",
            "If you could master any skill instantly, what would it be?",
            "What book has influenced you the most?",
            "What do you value most in a friendship?",
            "If you had to describe yourself in three words, what would they be?",
            "What is your idea of a perfect day?",
            "If you could relive one day from your past, which one would it be?",
            "What’s your favorite way to spend a weekend?",
            "If you could have dinner with any fictional character, who would it be?",
            "What motivates you to keep going when times are tough?",
            "If you had to live in a movie universe, which one would you choose?",
            "What is one thing you wish you knew earlier in life?",
            "If you could switch lives with someone for a day, who would it be?",
            "What’s your favorite quote or saying?",
            "If you had to give up one of your five senses, which would it be?",
            "What’s a hobby you’ve always wanted to try but never have?",
            "If you could send a message to your future self, what would it say?"
        };
}