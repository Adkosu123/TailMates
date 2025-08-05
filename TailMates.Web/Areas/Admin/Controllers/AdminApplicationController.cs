using Microsoft.AspNetCore.Mvc;
using TailMates.Data.Models.Enums;
using TailMates.Services.Core.Interfaces;
using TailMates.Web.ViewModels.Admin;

namespace TailMates.Web.Areas.Admin.Controllers
{
	public class AdminApplicationController : BaseAdminController
    {
		private readonly IAdminService adminService;
		private readonly ILogger<AdminApplicationController> logger;

		public AdminApplicationController(
			IAdminService adminService,
			ILogger<AdminApplicationController> logger)
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
				var applications = await adminService.GetAllApplicationsAsync(pageIndex, PageSize);
				var viewModel = new AdoptionApplicationListViewModel
				{
					Applications = applications
				};

				return View(viewModel);
			}
			catch (Exception e)
			{
				logger.LogError(e.Message);
				return RedirectToAction("Index", "Home");
			}
		}

		[HttpGet]
		public async Task<IActionResult> Details(int id)
		{
			try
			{
				var applicationDetails = await adminService.GetApplicationDetailsAsync(id);

				if (applicationDetails == null)
				{
					return NotFound();
				}

				return View(applicationDetails);
			}
			catch (Exception e)
			{
				logger.LogError(e.Message);
				return RedirectToAction("Error", "Home");
			}
		}

		[HttpPost]
		public async Task<IActionResult> UpdateApplicationStatus(int id, ApplicationStatus status, string adminNotes)
		{
			try
			{
				var success = await adminService.UpdateApplicationStatusAndNotesAsync(id, status, adminNotes);

				if (!success)
				{
					return NotFound();
				}

				TempData["SuccessMessage"] = "Application status and notes updated successfully.";
				return RedirectToAction(nameof(Index), "AllApplications");
			}
			catch (Exception e)
			{
				logger.LogError(e.Message);
				return RedirectToAction("Error", "Home");
			}
		}
	}
}
