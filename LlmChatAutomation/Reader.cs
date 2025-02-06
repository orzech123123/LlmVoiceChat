using System;
using System.Globalization;
using System.IO;
using System.Speech.Synthesis;

namespace LlmChatAutomation
{
    public class Reader
    {
        public byte[] ReadText(bool pl, string text, bool? saveToFile = false)
        {
            using (var synthesizer = new SpeechSynthesizer())
            {
                synthesizer.SetOutputToDefaultAudioDevice();

                if (pl)
                {
                    synthesizer.SelectVoiceByHints(VoiceGender.Male, VoiceAge.Adult, 0, CultureInfo.GetCultureInfo("pl-PL"));
                }
                else
                {
                    synthesizer.SelectVoiceByHints(VoiceGender.Male, VoiceAge.Adult, 0, CultureInfo.GetCultureInfo("en-US"));
                }

                if (saveToFile == true)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        synthesizer.SetOutputToWaveStream(memoryStream);
                        synthesizer.Speak(text);
                        //memoryStream.Seek(0, SeekOrigin.Begin);
                        return memoryStream.ToArray();
                    }
                }
                else
                {
                    synthesizer.Speak(text);
                    return null;
                }
            }
        }
    }
}
