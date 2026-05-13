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
            // Must match FastAPI Pydantic field names (camelCase "question"), not C# PascalCase.
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var json = JsonSerializer.Serialize(request, jsonOptions);

            var response = await _http.PostAsync(
                "http://10.0.4.173:8000/ask",
                new StringContent(json, Encoding.UTF8, "application/json")
            );

            var result = await response.Content.ReadAsStringAsync();

            return Content(result, "application/json");
        }
    }

    public class ChatRequest
    {
        public string Question { get; set; }

    }

}