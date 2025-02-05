using LlmChatAutomation;
using Microsoft.AspNetCore.Mvc;

namespace LlmChatApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly ILogger<ChatController> _logger;

        public ChatController(ILogger<ChatController> logger)
        {
            _logger = logger;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadAudioFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "recorded.mp3");

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var transcriber = new Transcriber();
                var transcription = transcriber.TranscribeVoice(false);

                return Ok("File uploaded successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet]
        public IActionResult Get()
        {
            return Ok();
        }
            //var chat = new Chat();

            //await chat.InitAsync();

            //chat.Start();

            //Thread.Sleep(2000);

            //chat.SendPrompt("hi!");

            //Thread.Sleep(2000);
    }
}
