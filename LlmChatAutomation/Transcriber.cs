using System.Diagnostics;

namespace LlmChatAutomation
{
    public class Transcriber
    {
        public string TranscribeVoice(bool pl, string filename)
        {
            var language = pl ? "Polish" : "English";

            string exePath = @"C:\\Projects\\Faster-Whisper-XXL_r192.3.4_windows\\Faster-Whisper-XXL\\faster-whisper-xxl.exe";
            string arguments = $@".\{filename}.mp3 --language {language} --model medium --output_dir source";

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

            string filePath = $"{filename}.srt";

            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);

                return lines[2];
            }

            return null;
        }
    }
}
