using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ResumeSystem.Models.Database;

namespace ResumeSystem.Controllers
{
	public class AccountController : Controller
	{
		private readonly SignInManager<User> _signInManager;

		public AccountController(SignInManager<User> signInManager)
		{
			_signInManager = signInManager;
		}
		[HttpPost]
		public async Task<IActionResult> SignIn(string email, string password)
		{
			var result = await _signInManager.PasswordSignInAsync(email, password, false, lockoutOnFailure: false);
			if (result.Succeeded)
			{
				return RedirectToAction("Index", "Home");
			}

			ModelState.AddModelError("", "Invalid login attempt.");
			return View();
		}

		[HttpGet]
		public IActionResult SignIn()
		{
			return View();
		}
	}
}
