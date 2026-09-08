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
        private readonly TtsService _ttsService;
        private readonly ILogger<TTSController> _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="openRouterService">OpenRouter service</param>
        /// <param name="ttsService">Text-to-speech service</param>
        /// <param name="logger">Logger implementation</param>
        public TTSController(OpenRouterService openRouterService, TtsService ttsService, ILogger<TTSController> logger)
        {
            _openRouterService = openRouterService;
            _ttsService = ttsService;
            _logger = logger;
        }

        /// <summary>
        /// Gets an image from request body, encodes it in base64 and sends it to OpenRouter for processing, then returns the TTS result.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/tts/fromimage")]
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

            // return 422: unprocessable content when model on OpenRouter returns ERROR_NO_TEXT
            if (imgText.ToUpper().Contains("ERROR_NO_TEXT"))
            {
                return StatusCode(422, "No text found.");
            }

            byte[] audioBytes;

            try
            {
                audioBytes = await _ttsService.Generate(imgText);
            }
            catch (Exception e)
            {
                _logger.LogError("Exception occurred while generating speech.\n{message}", e.Message);
                return StatusCode(500, "Error occurred while generating speech.");
            }
            

            return File(audioBytes, "audio/wav", "output.wav");
        }

        /// <summary>
        /// Gets an image from request body, encodes it in base64 and sends it to OpenRouter for processing, then returns the transcribed text (for testing).
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/tts/transcribe")]
        public async Task<IActionResult> Transcribe()
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

            return Ok(new
            {
                text = imgText
            });
        }

        /// <summary>
        /// Gets a string from the request body, sends it to the TTS service for processing, then returns the generated audio bytes (for testing).
        /// </summary>
        /// <param name="text">String text to convert to speech</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/tts/fromtext")]
        public async Task<IActionResult> TTSFromText(string msg)
        {
            _logger.Log(LogLevel.Information, "Generating speech from text: {text}", msg);
            try
            {
                byte[] audioBytes = await _ttsService.Generate(msg);
                return File(audioBytes, "audio/wav", "output.wav");

            }
            catch (Exception e)
            {
                _logger.LogError("Exception occurred while generating speech.\n{message}", e.Message);
                return StatusCode(500, "Error occurred while generating speech.");
            }
        }
    }
}
