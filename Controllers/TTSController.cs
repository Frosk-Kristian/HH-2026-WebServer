//using Microsoft.AspNetCore.Http;
using HH_2026_WebServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HH_2026_WebServer.Controllers
{
    /// <summary>
    /// Controller for handling Text-to-Speech requests. Sole controller that an external client will interact with.
    /// </summary>
    /// <remarks>
    /// To-Do: Implement text to speech functionality, currently transcribes text and returns it
    /// </remarks>
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
        [Route("api/tts/transcribe")]
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
