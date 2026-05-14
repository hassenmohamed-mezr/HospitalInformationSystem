using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace HospitalInformationSystem.Controllers
{
    public class ChatController : Controller
    {
        private readonly HttpClient _http;

        public ChatController(IHttpClientFactory factory)
        {
            _http = factory.CreateClient();
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Ask([FromBody] ChatRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Question))
            {
                return Content(
                    JsonSerializer.Serialize(new { answer = "Question is required." }),
                    "application/json",
                    Encoding.UTF8
                );
            }

            // Must match FastAPI Pydantic field names (camelCase "question"), not C# PascalCase.
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var json = JsonSerializer.Serialize(request, jsonOptions);

            try
            {
                var response = await _http.PostAsync(
                    "http://127.0.0.1:8000/ask",
                    new StringContent(json, Encoding.UTF8, "application/json")
                );

                var result = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(result))
                {
                    result = JsonSerializer.Serialize(new { answer = "Assistant returned an empty response." }, jsonOptions);
                }

                return new ContentResult
                {
                    Content = result,
                    ContentType = "application/json",
                    StatusCode = (int)response.StatusCode
                };
            }
            catch (Exception)
            {
                var errorJson = JsonSerializer.Serialize(new { answer = "Unable to reach assistant service." }, jsonOptions);
                return new ContentResult
                {
                    Content = errorJson,
                    ContentType = "application/json",
                    StatusCode = 503
                };
            }
        }
    }

    public class ChatRequest
    {
        public string Question { get; set; }

    }

}