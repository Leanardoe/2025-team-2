using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OpenAI;
using ResumeSystem.Models;
using ResumeSystem.Models.Database;

namespace ResumeSystem.Controllers
{
    public class ResumeController : Controller
    {
        private readonly ResumeContext _context;
        private readonly OpenAIClient _client;
        private readonly string _prompt;

        public ResumeController(IConfiguration config, ResumeContext ctx)
        {
            _context = ctx;
            _client = new OpenAIClient(config["OpenAI:ApiKey"]);
            _prompt = config["AI:Prompt"];
        }

        private bool IsUserLoggedIn()
        {
            return HttpContext.Session.GetString("UserEmail") != null;
        }

        public IActionResult Uploading()
        {
            if (!IsUserLoggedIn()) return RedirectToAction("SignIn", "Home");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Uploading(IFormFile resumeFile)
        {
            if (!IsUserLoggedIn()) return RedirectToAction("SignIn", "Home");

            if (resumeFile == null || resumeFile.Length == 0)
            {
                ViewBag.Error = "No file uploaded.";
                return View();
            }

            var result = await AIProcess.ProcessResumeAsync(resumeFile, _prompt, _client);

            if (result.Correct)
            {
                string fileName = Path.GetFileNameWithoutExtension(resumeFile.FileName);
                FileUpload fileUpload = new FileUpload(_context);
                fileUpload.ResumeUpload(fileName, result.Text);
                ViewBag.Message = "Resume processed and skills saved successfully.";
            }
            else
            {
                ViewBag.Error = result.Text;
            }

            return View();
        }

        public IActionResult MassUploading()
        {
            if (!IsUserLoggedIn()) return RedirectToAction("SignIn", "Home");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> MassUploading(List<IFormFile> resumeFiles)
        {
            if (!IsUserLoggedIn()) return RedirectToAction("SignIn", "Home");

            foreach (var resumeFile in resumeFiles)
            {
                if (resumeFile == null || resumeFile.Length == 0) continue;

                var result = await AIProcess.ProcessResumeAsync(resumeFile, _prompt, _client);

                if (result.Correct)
                {
                    string fileName = Path.GetFileNameWithoutExtension(resumeFile.FileName);
                    FileUpload fileUpload = new FileUpload(_context);
                    fileUpload.ResumeUpload(fileName, result.Text);
                }
            }

            ViewBag.Message = "All resumes processed.";
            return View();
        }

        public IActionResult Filtering()
        {
            if (!IsUserLoggedIn()) return RedirectToAction("SignIn", "Home");
            return View();
        }
    }
}
