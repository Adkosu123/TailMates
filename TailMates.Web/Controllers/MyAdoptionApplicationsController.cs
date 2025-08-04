using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TailMates.Services.Core.Interfaces;
using TailMates.Web.ViewModels.MyAdoptionApplications;

namespace TailMates.Web.Controllers
{
	[Authorize]
	public class MyAdoptionApplicationsController : BaseController
	{
		private readonly IMyAdoptionApplicationsService myAdoptionApplicationsService;
		private readonly ILogger<MyAdoptionApplicationsController> logger;

		public MyAdoptionApplicationsController(
			IMyAdoptionApplicationsService myAdoptionApplicationsService,
			ILogger<MyAdoptionApplicationsController> logger)
		{
			this.myAdoptionApplicationsService = myAdoptionApplicationsService;
			this.logger = logger;
		}

		[HttpGet]
		public async Task<IActionResult> All(int pageIndex = 1)
		{
			const int PageSize = 6;

			try
			{
				var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
				if (string.IsNullOrEmpty(userId))
				{
					return RedirectToPage("/Account/Login", new { area = "Identity" });
				}

				var paginatedApplications = await this.myAdoptionApplicationsService.GetUserApplicationsAsync(userId, pageIndex, PageSize);

				var viewModel = new MyApplicationsListViewModel
				{
					Applications = paginatedApplications
				};

				return View(viewModel);
			}
			catch (Exception e)
			{
				this.logger.LogError(e.Message);
				TempData["ErrorMessage"] = "An error occurred while loading your applications. Please try again.";
				return this.RedirectToAction("Index", "Home");
			}
		}
	}
}
