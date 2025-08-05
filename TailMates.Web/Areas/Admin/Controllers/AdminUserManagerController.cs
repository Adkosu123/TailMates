using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TailMates.Data.Models;
using TailMates.Services.Core.Interfaces;
using TailMates.Web.ViewModels.Admin;

namespace TailMates.Web.Areas.Admin.Controllers
{
	public class AdminUserManagerController : BaseAdminController
	{
		private readonly IAdminService adminService;
		private readonly UserManager<ApplicationUser> userManager;

		public AdminUserManagerController(IAdminService adminService, UserManager<ApplicationUser> userManager)
		{
			this.adminService = adminService;
			this.userManager = userManager;
		}

		[HttpGet]
		public async Task<IActionResult> AllUsers(int pageIndex = 1)
		{
			const int PageSize = 10;
			var users = await this.adminService.GetAllUsersAsync(pageIndex, PageSize);
			var viewModel = new AllUsersListViewModel
			{
				Users = users
			};
			return View(viewModel);
		}

		[HttpGet]
		public async Task<IActionResult> ManageUserRoles(string id)
		{
			var viewModel = await this.adminService.GetUserRolesAndShelterAsync(id);
			if (viewModel == null)
			{
				return NotFound();
			}
			return View(viewModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ManageUserRoles(ManageUserRolesViewModel model)
		{
			if (!ModelState.IsValid)
			{
				var reloadedModel = await this.adminService.GetUserRolesAndShelterAsync(model.UserId);
				if (reloadedModel != null)
				{
					model.AvailableRoles = reloadedModel.AvailableRoles;
					model.AllShelters = reloadedModel.AllShelters;
				}
				return View(model);
			}

			var success = await this.adminService.UpdateUserRolesAndShelterAsync(model.UserId, model.SelectedRoles, model.ManagedShelterId);

			if (success)
			{
				TempData["SuccessMessage"] = $"Roles and Managed Shelter for {model.Username} updated successfully.";
				return RedirectToAction(nameof(AllUsers));
			}
			else
			{
				TempData["ErrorMessage"] = $"Failed to update roles and Managed Shelter for {model.Username}.";
				return RedirectToAction(nameof(ManageUserRoles), new { id = model.UserId });
			}
		}
	}
}