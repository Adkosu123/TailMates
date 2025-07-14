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
		private readonly TailMatesDbContext _dbContext;

		public ApplicationDbInitializer(
			UserManager<ApplicationUser> userManager,
			RoleManager<IdentityRole> roleManager,
			TailMatesDbContext dbContext)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_dbContext = dbContext;
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

		public async Task SeedManagerUserAsync()
		{
			const string managerEmail = "manager@happypaws.com";
			const string managerPassword = "ManagerPassword123!"; 
			const string shelterName = "Happy Paws Shelter"; 

			var managerUser = await _userManager.FindByEmailAsync(managerEmail);

			if (managerUser == null)
			{
				managerUser = new ApplicationUser
				{
					UserName = managerEmail,
					Email = managerEmail,
					EmailConfirmed = true,
					FirstName = "Shelter",
					LastName = "Manager"
				};

				var result = await _userManager.CreateAsync(managerUser, managerPassword);

				if (result.Succeeded)
				{
					await _userManager.AddToRoleAsync(managerUser, "Manager");
					var shelter = await _dbContext.Shelters.FirstOrDefaultAsync(s => s.Name == shelterName);
					if (shelter != null)
					{
						managerUser.ManagedShelterId = shelter.Id;
						await _userManager.UpdateAsync(managerUser);
					}
				}
			}
		}
	}
}
