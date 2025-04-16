using Microsoft.AspNetCore.Mvc;

namespace ResumeSystem.Controllers
{
    public class HomeController : Controller
    {
        // Placeholder: Disable session check for now
        private bool IsUserLoggedIn() => true;

        public IActionResult Index()
        {
            // Directly go to Uploading page for now
            return RedirectToAction("Uploading", "Resume");
        }

        public IActionResult SignIn() => View();

        public IActionResult Test() => View();

        [HttpPost]
        public IActionResult SignIn(string email, string password)
        {
            // Placeholder: Always treat login as successful
            // You can re-enable this logic when session is configured:
            // HttpContext.Session.SetString("UserEmail", email);

            return RedirectToAction("Uploading", "Resume");
        }

        public IActionResult SignOut()
        {
            // Placeholder: no-op
            // HttpContext.Session.Clear();
            return RedirectToAction("SignIn");
        }

        public IActionResult NavPage()
        {
            // Placeholder: always allow access
            return View();
        }
    }
}
