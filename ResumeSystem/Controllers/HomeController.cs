using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using OpenAI;
using OpenAI.Chat;
using ResumeSystem.Models;
using ResumeSystem.Models.Database;
using System.Text;

namespace ResumeSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly OpenAIClient _client;
        private readonly string _prompt;

		private ResumeContext context;

		public HomeController(IConfiguration config, ResumeContext ctx)
        {
			context = ctx;
			var apiKey = config["OpenAI:ApiKey"];
            var apiKey = config["OpenAI:ApiKey"];
            _prompt = config["AI:Prompt"]; 
            _client = new OpenAIClient(apiKey);
            _prompt = config["AI:Prompt"];
        }

        public IActionResult Index() => View();

        public IActionResult Test() => View();

        [HttpPost]
        public async Task<IActionResult> Test(IFormFile resumeFile)
        {
            if (false) //block and not use tokens while testing
            {
                if (resumeFile == null || resumeFile.Length == 0)
                {
                    ViewBag.Result = "❌ Please upload a valid .txt resume file.";
                    return View();
                }

                string resumeText;
                try
                {
                    using var reader = new StreamReader(resumeFile.OpenReadStream(), Encoding.UTF8);
                    resumeText = await reader.ReadToEndAsync();

                    if (string.IsNullOrWhiteSpace(resumeText))
                    {
                        ViewBag.Result = "❌ The uploaded file appears to be empty.";
                        return View();
                    }
                }
                catch (Exception readEx)
                {
                    ViewBag.Result = $"❌ Failed to read uploaded file: {readEx.Message}";
                    return View();
                }

            try
            {
                var chatRequest = new ChatRequest(
                    new[]
                    {
                        new Message(Role.System, "You are a resume skill extractor."),
                        new Message(Role.User, _prompt)
                    },
                    model: "gpt-3.5-turbo",
                    //model: "gpt-4o", (switch to 4o for production tests)
                    temperature: 0.4,
                    maxTokens: 500
                );

                    var response = await _client.ChatEndpoint.GetCompletionAsync(chatRequest);

                    var resultContent = response?.FirstChoice?.Message?.Content?.ToString();

                    if (string.IsNullOrWhiteSpace(resultContent))
                    {
                        ViewBag.Result = "⚠️ OpenAI returned no content. The response was empty.";
                    }
                    else
                    {
                        ViewBag.Result = resultContent;
                    }
                }
                catch (HttpRequestException ex)
                {
                    ViewBag.Result = $"❌ OpenAI HTTP error: {ex.Message}";
                }
                catch (Exception ex)
                {
                    ViewBag.Result = $"❌ Unexpected error: {ex.Message}";
                }
            }

            FileUpload fileUpload = new FileUpload(context);

            fileUpload.ResumeUpload("wertyuisdfghjsdfgerty.", "...");

            return View();
        }
    }
}
