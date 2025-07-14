using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore; 
using System.Threading.Tasks;
using System.Linq;
using TailMates.Data.Models; 

namespace TailMates.Data.Seed
{

	public class ApplicationDbInitializer
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;

		public ApplicationDbInitializer(
			UserManager<ApplicationUser> userManager,
			RoleManager<IdentityRole> roleManager)
		{
			_userManager = userManager;
			_roleManager = roleManager;
		}

		public async Task SeedRolesAndUsersAsync()
		{
			await SeedRoleAsync("Admin");
			await SeedRoleAsync("Manager");
			await SeedRoleAsync("User"); 

			await SeedAdminUserAsync();
		}

		private async Task SeedRoleAsync(string roleName)
		{
			if (!await _roleManager.RoleExistsAsync(roleName))
			{
				_ = await _roleManager.CreateAsync(new IdentityRole(roleName));
			}
		}

		private async Task SeedAdminUserAsync()
		{
			const string adminEmail = "admin@tailmates.com";
			const string adminPassword = "AdminPassword123!";

			var adminUser = await _userManager.FindByEmailAsync(adminEmail);

			if (adminUser == null)
			{
				adminUser = new ApplicationUser
				{
					UserName = adminEmail, 
					Email = adminEmail,
					EmailConfirmed = true,
					FirstName = "System",
					LastName = "Admin"
				};

				var result = await _userManager.CreateAsync(adminUser, adminPassword);

				if (result.Succeeded)
				{
					
					await _userManager.AddToRoleAsync(adminUser, "Admin");
				}
				else
				{ 
					foreach (var error in result.Errors)
					{
						Console.WriteLine($"Error creating admin user: {error.Description}");
					}
				}
			}
		}
	}
}
