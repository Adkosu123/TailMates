using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using TailMates.Data.Models;
using TailMates.Services.Core.Interfaces;
using TailMates.Services.Core.Services;
using TailMates.Web.ViewModels.Shelter;

namespace TailMates.Web.Controllers
{
    public class ShelterController : BaseController
    {
        private readonly IShelterService shelterService;
		private readonly ILogger<ShelterController> logger;
		private readonly UserManager<ApplicationUser> userManager;
		public ShelterController(IShelterService shelterService,
			ILogger<ShelterController> logger,
			UserManager<ApplicationUser> userManager)
		{
            this.shelterService = shelterService;
			this.logger = logger;
			this.userManager = userManager;
		}

		[HttpGet]
		public async Task<IActionResult> All(int pageIndex = 1)
		{
			const int PageSize = 6;

			try
			{
				var paginatedShelters = await this.shelterService.GetAllSheltersAsync(pageIndex, PageSize);

				var viewModel = new ShelterListViewModel
				{
					Shelters = paginatedShelters
				};

				return View(viewModel);
			}
			catch (Exception e)
			{
				this.logger.LogError(e, "An error occurred while fetching shelters.");
				return this.RedirectToAction("Index", "Home");
			}
		}

		[HttpGet]
		public async Task<IActionResult> Details(int id, int pageIndex = 1)
		{
			const int PageSize = 6;
			try
			{
				var userManagedShelterId = 0;
				var currentUserId = this.userManager.GetUserId(this.User);
				var currentUser = await this.userManager.FindByIdAsync(currentUserId);

				if (currentUser != null && (await this.userManager.IsInRoleAsync(currentUser, "Admin") || await this.userManager.IsInRoleAsync(currentUser, "Manager")))
				{
					userManagedShelterId = currentUser.ManagedShelterId ?? 0;
				}

				var viewModel = await this.shelterService.GetShelterDetailsWithPaginatedPetsAsync(id, pageIndex, PageSize);

				if (viewModel == null)
				{
					return this.NotFound();
				}

				this.ViewBag.UserManagedShelterId = userManagedShelterId;
				return this.View(viewModel);
			}
			catch (Exception e)
			{
				this.logger.LogError(e, "An error occurred while fetching shelter details for ID {ShelterId}", id);
				return this.RedirectToAction("All");
			}
		}

		[HttpGet]
		[Authorize(Roles = "Admin")] 
		public IActionResult Add()
		{
			return View();
		}

		[HttpPost]
		[Authorize(Roles = "Admin")] 
		public async Task<IActionResult> Add(ShelterCreateViewModel model)
		{

			try
			{
				if (!ModelState.IsValid)
				{
					TempData["Message"] = "Validation failed. Please check your inputs.";
					TempData["MessageType"] = "error";
					return View(model);
				}

				bool success = await shelterService.AddShelterAsync(model);

				if (success)
				{
					TempData["Message"] = $"Shelter '{model.Name}' added successfully!";
					TempData["MessageType"] = "success";
					return RedirectToAction("All", "Shelter");
				}
				else
				{
					TempData["Message"] = $"Failed to add shelter '{model.Name}'. Please try again.";
					TempData["MessageType"] = "error";
					return View(model);
				}
			}
			catch (Exception e)
			{
				this.logger.LogError(e.Message);
				return this.RedirectToAction("All", "Shelter");
			}
			
		}
	}
}
