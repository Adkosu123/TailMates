using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TailMates.Services.Core.Interfaces;
using TailMates.Web.ViewModels.Admin;

namespace TailMates.Web.Controllers
{
	[Authorize(Roles = "Admin")]
	public class AdminController : BaseController
	{
		private readonly IAdminService adminService;
		private readonly ILogger<AdminController> logger;

		public AdminController(IAdminService adminService,
			ILogger<AdminController> logger)
		{
			this.adminService = adminService;
			this.logger = logger;
		}

		[HttpGet]
		public async Task<IActionResult> AllApplications(int pageIndex = 1)
		{
			try
			{
				const int PageSize = 6;
				var applications = await this.adminService.GetAllApplicationsAsync(pageIndex, PageSize);
				var viewModel = new AdoptionApplicationListViewModel
				{
					Applications = applications
				};
				return View(viewModel);
			}
			catch (Exception e)
			{
				this.logger.LogError(e.Message);
				return this.RedirectToAction("Error", "Home");
			}
		}

		[HttpGet]
		public async Task<IActionResult> Details(int id)
		{
			try
			{
				var applicationDetails = await this.adminService.GetApplicationDetailsAsync(id);

				if (applicationDetails == null)
				{
					return NotFound();
				}

				return View(applicationDetails);
			}
			catch (Exception e)
			{
				this.logger.LogError(e.Message);
				return this.RedirectToAction("Error", "Home");
			}
		}
	}
}
