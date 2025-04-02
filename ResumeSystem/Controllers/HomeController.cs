using Microsoft.AspNetCore.Mvc;
using ResumeSystem.Models;


namespace ResumeSystem.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Test(QuerySearch model)
        {
           
            List<string> keywords = new List<string>() {"apple"};


            if (ModelState.IsValid)
            {
                ViewBag.Fv = model.KeywordSearch(keywords);
            }
            else
            {
                ViewBag.Fv = "not working";
            }


                return View(model);
        }
    }
}
