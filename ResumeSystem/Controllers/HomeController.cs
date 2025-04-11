using Microsoft.AspNetCore.Mvc;

namespace ResumeSystem.Controllers
{
    public class HomeController : Controller
    {
        // Home landing logic
        public IActionResult Index()
        {
            if (!IsUserLoggedIn())
                return RedirectToAction("SignIn");

            return RedirectToAction("Uploading", "Resume");
        }

        // Login page
        public IActionResult SignIn() => View();

        [HttpPost]
        public IActionResult SignIn(string email, string password)
        {
            // Example login logic
            if (email == "admin@example.com" && password == "password")
            {
                HttpContext.Session.SetString("UserEmail", email);
                return RedirectToAction("Uploading", "Resume");
            }

            ViewBag.ErrorMessage = "Invalid credentials.";
            return View();
        }

        public IActionResult SignOut()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("SignIn");
        }

        // Optional page with nav previews or hub
        public IActionResult NavPage()
        {
            if (!IsUserLoggedIn())
                return RedirectToAction("SignIn");

            return View();
        }

        private bool IsUserLoggedIn()
        {
            return HttpContext.Session.GetString("UserEmail") != null;
        }
    }
}
