using Microsoft.AspNetCore.Mvc;
using OpenAI;
using ResumeSystem.Models;
using ResumeSystem.Models.Database;

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

            if (result.Correct) //carry on if AI API did not fail
            {
                FileUpload fileUpload = new FileUpload(context);
                
                //TODO we will determine where the resume is stored later
                fileUpload.ResumeUpload("aaaaaaaaaaaaaaaaaa", result.Text, result.ResumeBody);
            }

            return View();
        }
    }
}
