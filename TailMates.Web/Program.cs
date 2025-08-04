using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TailMates.Data;
using TailMates.Data.Models;
using TailMates.Data.Repositories.Interfaces;
using TailMates.Data.Seed;
using TailMates.Services.Core.Interfaces;
using TailMates.Services.Core.Services;
using TailMates.Web.Infrastructure.Extensions;

namespace TailMates.Web
{

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

			builder.Services.ConfigureApplicationCookie(options =>
			{
				options.AccessDeniedPath = "/Home/AccessDenied";
			});

			builder.Services.AddRepositories(typeof(IPetRepository).Assembly);

			builder.Services.AddUserDefinedServices(typeof(IPetService).Assembly);

			builder.Services.AddScoped<IPetService, PetService>();

			builder.Services.AddControllersWithViews();

			builder.Services.AddRazorPages();

			builder.Services.AddScoped<ApplicationDbInitializer>();

			WebApplication? app = builder.Build();

			using (var scope = app.Services.CreateScope())
			{
				var services = scope.ServiceProvider;
				var initializer = services.GetRequiredService<ApplicationDbInitializer>();
				await initializer.SeedRolesAndUsersAsync();
				await initializer.SeedManagerUserAsync();

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
			//app.UseStatusCodePagesWithRedirects("/Home/Error?statusCode={0}");

			app.UseAuthentication();
			app.UseAuthorization();

			app.MapControllerRoute(
				name: "areas",
				pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");
			app.MapRazorPages();

			app.Run();
		}
	}
}