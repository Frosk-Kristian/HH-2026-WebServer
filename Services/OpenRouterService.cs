using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace HH_2026_WebServer.Services
{
    /// <summary>
    /// Service class for interacting with OpenRouter's API.
    /// </summary>
    public class OpenRouterService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        /// <summary>
        /// Constructor for OpenRouterService, initializes HttpClient and retrieves API key from configuration.
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="configuration"></param>
        /// <exception cref="InvalidOperationException">If API Key has not been configured in appsettings.json</exception>
        public OpenRouterService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["OpenRouter:ApiKey"] ?? throw new InvalidOperationException("API Key is not configured!");
        }

        /// <summary>
        /// Transcribes text from an image by sending it to OpenRouter's API.
        /// </summary>
        /// <param name="imgBytes"></param>
        /// <returns></returns>
        public async Task<string> TranscribeImage(byte[] imgBytes)
        {
            // encodes image bytes to base64
            string base64Image = Convert.ToBase64String(imgBytes);
            // constructs the data URL for the image, for our purposes the image is always a JPEG
            string imgUrl = $"data:image/jpeg;base64,{base64Image}";

            // constructs the request body to be sent to OpenRouter's API
            var requestBody = new {
                model = "google/gemma-4-31b-it",
                messages = new[] {
                    new {
                        role = "user",
                        content = new object[] {
                            new {
                                type = "text",
                                text = "Transcribe the text in this image"
                            },
                            new {
                                type = "image_url",
                                image_url = new {
                                    url = imgUrl
                                }
                            }
                        }
                    }
                },
                provider = new {
                    only = new[]
                    {
                        "cerebras"
                    }
                }
            };

            // serializes the request body to JSON
            string json = JsonSerializer.Serialize(requestBody);

            // sends request to OpenRouter
            using var request = new HttpRequestMessage(HttpMethod.Post, "https://openrouter.ai/api/v1/chat/completions");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            // awaits response from OpenRouter, throwing an exception if the response indicates failure
            using HttpResponseMessage response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            // reads and parses response content
            string responseContent = await response.Content.ReadAsStringAsync();
            using JsonDocument document = JsonDocument.Parse(responseContent);
            string result = document.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString() ?? "";

            return result;
        }
    }
}
