using Microsoft.AspNetCore.Mvc;

namespace ResumeSystem.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // Directly go to Uploading page for now
            //return RedirectToAction("Uploading", "Resume");
            return View();
        }
    }
}
