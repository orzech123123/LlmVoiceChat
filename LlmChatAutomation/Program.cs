using LlmChatAutomation;
using NAudio.Lame;
using NAudio.Wave;

class Program
{
    static bool _pl = true;

    static WaveInEvent waveSource;
    static WaveFileWriter waveFile;
    static string outputWavFile = "recorded.wav";
    static string outputMp3File = "recorded.mp3";

    static async Task Main()
    {
        var reader = new Reader();
        var transcriber = new Transcriber();
        var chat = new Chat();

        await chat.InitAsync();

        chat.Start();

        Thread.Sleep(2000);

        while (true)
        {
            chat.SkipLogoutPopup();

            var filename = Guid.NewGuid().ToString();
            outputWavFile = $"{filename}.wav";
            outputMp3File = $"{filename}.mp3";

            RecordVoice();
            var prompt = transcriber.TranscribeVoice(_pl, filename);

            chat.SendPrompt(prompt);

            Thread.Sleep(3000);

            var answer = chat.GetAnswer();

            reader.ReadText(_pl, answer);
        }
    }

    //below, move to Recorder.cs

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

    //------- to Recorder.cs ---------
}