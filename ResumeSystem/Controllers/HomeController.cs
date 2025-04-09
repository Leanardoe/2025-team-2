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
            _client = new OpenAIClient(apiKey);
            _prompt = config["AI:Prompt"];
        }

        public IActionResult Index() => View();

        public IActionResult Test() => View();

        [HttpPost]
        public async Task<IActionResult> Test(IFormFile resumeFile)
        {
            var result = await AIProcess.ProcessResumeAsync(resumeFile,_prompt,_client);

            if (result.Correct)
            {
                FileUpload fileUpload = new FileUpload(context);

                fileUpload.ResumeUpload("aaaaaaaaaaaaaaaaaa.", result.Text);
            }

            return View();
        }
    }
}
