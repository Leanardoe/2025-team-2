﻿using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OpenAI;
using ResumeSystem.Models;
using ResumeSystem.Models.Database;
using ResumeSystem.Services;

namespace ResumeSystem.Controllers
{
    public class ResumeController : Controller
    {
        private readonly ResumeContext _context;
        private readonly OpenAIClient _client;
        private readonly string _prompt;
        private readonly BlobService _blobService;

        public ResumeController(IConfiguration config, ResumeContext ctx, BlobService blobService)
        {
            _context = ctx;
            _client = new OpenAIClient(config["OpenAI:ApiKey"]);
            _prompt = config["AI:Prompt"];
            _blobService = blobService;
        }

        [HttpGet]
        [Authorize]
        public IActionResult Uploading()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Uploading(IFormFile resume, string? name, string? email, string? phone)
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
                    string blobName = await _blobService.UploadResumeAsync(resume, "resumes");
                    string blobUrl = _blobService.GenerateDownloadSasUri("resumes", blobName, TimeSpan.FromMinutes(10));

                    
                    if (!string.IsNullOrWhiteSpace(name))
                    {
						Candidate? existingCan = null;
						existingCan = _context.Candidates.FirstOrDefault(c =>
                            c.CAN_NAME == name && c.CAN_PHONE == phone && c.CAN_EMAIL == email);
                        if (existingCan == null)
                        {
                            existingCan = new Candidate() { CAN_NAME = name, CAN_PHONE = phone, CAN_EMAIL = email };
                        }
						FileUpload fileUpload = new FileUpload(_context);
						fileUpload.ResumeUpload(blobName, result.Text, result.ResumeBody, existingCan, blobUrl);
					}
                    else
                    {
						FileUpload fileUpload = new FileUpload(_context);
						fileUpload.ResumeUpload(blobName, result.Text, result.ResumeBody, blobUrl);
					}

                    ViewBag.UploadMSG = "Resume processed and skills saved successfully.";
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

        [HttpGet]
        [Authorize]
        public IActionResult MassUploading()
        {
            return Redirect(Url.Action("Uploading", "Resume") + "#multiUpload");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> MassUploading(List<IFormFile> resumes)
        {
            if (resumes == null || !resumes.Any())
            {
                TempData["ErrorMSG"] = 2;
                TempData["MassUploadMSG"] = "Failed to Process Resumes";
                return Redirect(Url.Action("Uploading", "Resume") + "#multiUpload");
            }

            var tasks = resumes.Select(async resume =>
            {
                var result = await AIProcess.ProcessResumeAsync(resume, _prompt, _client);
                return new { File = resume, Result = result };
            });

            int i = 0;
            var processed = await Task.WhenAll(tasks);
            FileUpload fileUpload = new FileUpload(_context);

            // Now you can loop through and use both file and result:
            foreach (var item in processed)
            {
                if (item.Result.Correct)
                {
                    string blobName = await _blobService.UploadResumeAsync(item.File, "resumes");
                    string blobUrl = _blobService.GenerateDownloadSasUri("resumes", blobName, TimeSpan.FromMinutes(10));

                    fileUpload.ResumeUpload(item.File.FileName, item.Result.Text, item.Result.ResumeBody, blobUrl);
                    i++;
                }
                else
                {
                    TempData["ErrorMSG"] = 0;
                }
            }

            TempData["MassUploadMSG"] = $"Processed {i} Resumes";
            if (i == 0)
            {
                TempData["ErrorMSG"] = 1;
                TempData["MassUploadMSG"] = "Failed to Process Resumes";
            }
            else
            {
                TempData["ErrorMSG"] = 0;
            }

            return Redirect(Url.Action("Uploading", "Resume") + "#multiUpload");
        }
    }
}
