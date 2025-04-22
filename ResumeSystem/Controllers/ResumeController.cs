using Microsoft.AspNetCore.Authorization;
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

        [HttpGet]
        [Authorize]
        public IActionResult Uploading()
        {
            return View();
        }

        [HttpPost]
		[Authorize]
		public async Task<IActionResult> Uploading(IFormFile resume, string name, string email)
        {
            try
            {
                if (resume == null)
                {
                    ViewBag.Error = "No file uploaded.";
                    return View();
                }

                var result = await AIProcess.ProcessResumeAsync(resume, _prompt, _client);

                if (result.Correct)
                {
                    string fileName = Path.GetFileNameWithoutExtension(resume.FileName);
                    FileUpload fileUpload = new FileUpload(_context);
                    fileUpload.ResumeUpload(fileName, result.Text, result.ResumeBody);
                    ViewBag.Message = "Resume processed and skills saved successfully.";
                }
                else
                {
                    ViewBag.Error = result.Text;
                }
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.ToString());
            }

            return View();
        }

		[Authorize]
		public IActionResult MassUploading()
        {
            return View();
        }

        [HttpPost]
		[Authorize]
		public async Task<IActionResult> MassUploading(List<IFormFile> resumeFiles)
        {

            foreach (var resumeFile in resumeFiles)
            {
                if (resumeFile == null || resumeFile.Length == 0) continue;

                var result = await AIProcess.ProcessResumeAsync(resumeFile, _prompt, _client);

                if (result.Correct)
                {
                    string fileName = Path.GetFileNameWithoutExtension(resumeFile.FileName);
                    FileUpload fileUpload = new FileUpload(_context);
                    fileUpload.ResumeUpload(fileName, result.Text, result.ResumeBody);
                }
            }

            ViewBag.Message = "All resumes processed.";
            return View();
        }

        public IActionResult Filtering()
        {
            return View();
        }
    }
}
