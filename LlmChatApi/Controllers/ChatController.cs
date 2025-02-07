using LlmChatAutomation;
using Microsoft.AspNetCore.Mvc;

namespace LlmChatApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatController : ControllerBase
    {
        private static Chat _chat;
        private bool _pl = false;

        private readonly ILogger<ChatController> _logger;

        public ChatController(ILogger<ChatController> logger)
        {
            _logger = logger;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadAudioFile(IFormFile file)
        {
            await InitChatAsync();

            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var filename = $"{Guid.NewGuid()}";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), $"{filename}.mp3");

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var reader = new Reader();
                var transcriber = new Transcriber();

                var prompt = transcriber.TranscribeVoice(false, filename);

                Thread.Sleep(2000);

                _chat.SendPrompt(prompt);

                Thread.Sleep(6000);

                var answer = _chat.GetAnswer();

                var outputAudio = reader.ReadText(_pl, answer, true);

                return File(outputAudio, "audio/wav", "output.wav");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            await InitChatAsync();

            return Ok();
        }

        private async Task InitChatAsync()
        {
            if (_chat == null)
            {
                _chat = new Chat();

                await _chat.InitAsync();

                _chat.Start();
            }
        }
    }
}
