using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TailMates.Data;
namespace TailMates.Web
{

	using Microsoft.AspNetCore.Identity;
	using Microsoft.EntityFrameworkCore;
	using TailMates.Data;
	using TailMates.Data.Models;
	using TailMates.Data.Seed;
	using TailMates.Services.Core.Interfaces;
	using TailMates.Services.Core.Services;

	public class Program
	{
		public static async Task Main(string[] args)
		{
			WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);

			string connectionString = builder
				.Configuration
				.GetConnectionString("DefaultConnection")
				?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

			builder.Services
				.AddDbContext<TailMatesDbContext>(options =>
				{
					options.UseSqlServer(connectionString);
				});
			builder.Services.AddDatabaseDeveloperPageExceptionFilter();
			builder.Services
				.AddDefaultIdentity<ApplicationUser>(options =>
				{
					options.SignIn.RequireConfirmedAccount = false;
					options.Password.RequireDigit = false;
					options.Password.RequireLowercase = false;
					options.Password.RequireUppercase = false;
					options.Password.RequireNonAlphanumeric = false;

				})
				.AddRoles<IdentityRole>()
				.AddEntityFrameworkStores<TailMatesDbContext>()
				.AddDefaultTokenProviders();

			builder.Services.AddScoped<IPetService, PetService>();
			builder.Services.AddScoped<IShelterService, ShelterService>();
			builder.Services.AddScoped<IAdoptionApplicationService, AdoptionApplicationService>();
			builder.Services.AddControllersWithViews();
			builder.Services.AddRazorPages();
			builder.Services.AddScoped<ApplicationDbInitializer>();

			WebApplication? app = builder.Build();

			using (var scope = app.Services.CreateScope())
			{
				var services = scope.ServiceProvider;
				var initializer = services.GetRequiredService<ApplicationDbInitializer>();
				await initializer.SeedRolesAndUsersAsync();

			}

			if (app.Environment.IsDevelopment())
			{
				app.UseMigrationsEndPoint();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");
			app.MapRazorPages();

			app.Run();
		}
	}
}