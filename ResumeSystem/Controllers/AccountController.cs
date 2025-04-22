using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ResumeSystem.Models.Database;

namespace ResumeSystem.Controllers
{
	public class AccountController : Controller
	{
		private SignInManager<User> _signInManager;
		private UserManager<User> _userManager;

		public AccountController(SignInManager<User> signInManager, UserManager<User> userManager)
		{
			_signInManager = signInManager;
			_userManager = userManager;
		}
		[HttpPost]
		public async Task<IActionResult> SignIn(string username, string password, bool rememberMe, string returnUrl = null)
		{
			var result = await _signInManager.PasswordSignInAsync(username, password, rememberMe, lockoutOnFailure: false);

			if (result.Succeeded)
			{
				if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
				{
					return Redirect(returnUrl);//go to page that forced signin
				}
				return RedirectToAction("Index", "Home");
			}

			ModelState.AddModelError("", "Invalid login attempt.");
			ViewData["ReturnUrl"] = returnUrl;
			return View();
		}

		[HttpGet]
		public IActionResult SignIn(string returnUrl = null)
		{
			ViewData["ReturnUrl"] = returnUrl;
			return View();
		}

		public async Task<IActionResult> SignOut()
		{
			await _signInManager.SignOutAsync();

			return RedirectToAction("Index", "Home");
		}
	}
}
