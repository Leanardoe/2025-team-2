using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using ResumeSystem.Models.Database;

namespace ResumeSystem.Models
{
	public static class IdentitySeeder
	{
		public static async Task SeedDefaultUserAsync(IServiceProvider serviceProvider)
		{
			var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
			var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
			var settings = serviceProvider.GetRequiredService<IOptions<DefaultAdminSettings>>().Value;

			if (!await roleManager.RoleExistsAsync(settings.Role))
			{
				await roleManager.CreateAsync(new IdentityRole(settings.Role));
			}

			var user = await userManager.FindByNameAsync(settings.Username);
			if (user == null)
			{
				user = new User
				{
					UserName = settings.Username,
					Email = settings.Email,
					EmailConfirmed = true
				};

				var result = await userManager.CreateAsync(user, settings.Password);
				if (result.Succeeded)
				{
					await userManager.AddToRoleAsync(user, settings.Role);
				}
				else
				{
					throw new Exception("Failed to create default user: " +
						string.Join(", ", result.Errors.Select(e => e.Description)));
				}
			}
		}
	}

	public class DefaultAdminSettings
	{
		public string Username { get; set; }
		public string Email { get; set; }
		public string Password { get; set; }
		public string Role { get; set; }
	}
}
