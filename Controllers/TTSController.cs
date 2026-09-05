//using Microsoft.AspNetCore.Http;
using HH_2026_WebServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HH_2026_WebServer.Controllers
{
    [ApiController]
    public class TTSController : ControllerBase
    {
        private readonly OpenRouterService _openRouterService;
        private readonly ILogger<TTSController> _logger;

        /// <summary>
        /// Gets an image from request body, encodes it in base64 and sends it to OpenRouter for processing, then returns the TTS result
        /// </summary>
        /// <returns></returns>

        [HttpPost]
        [Route("api/tts")]
        public async Task<IActionResult> TTSFromImage()
        {
            var memoryStream = new MemoryStream();
            await Request.Body.CopyToAsync(memoryStream);

            byte[] imageBytes = memoryStream.ToArray();

            string imgText = "";

            try
            {
                imgText = await _openRouterService.TranscribeImage(imageBytes);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError("OpenRouter API returned an error.\n{message}", e.Message);
                return StatusCode(500, "Error occurred while processing the request.");
            }

            return Ok(new {
                text = imgText
            });
        }
    }
}
