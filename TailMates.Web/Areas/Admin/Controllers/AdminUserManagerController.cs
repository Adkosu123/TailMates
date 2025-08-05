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
		private readonly ILogger<AdminUserManagerController> logger;

		public AdminUserManagerController(IAdminService adminService,
			UserManager<ApplicationUser> userManager
			,ILogger<AdminUserManagerController> logger)
		{
			this.adminService = adminService;
			this.userManager = userManager;
			this.logger = logger;
		}

		[HttpGet]
		public async Task<IActionResult> AllUsers(int pageIndex = 1)
		{
			try
			{
				const int PageSize = 10;
				var users = await this.adminService.GetAllUsersAsync(pageIndex, PageSize);
				var viewModel = new AllUsersListViewModel
				{
					Users = users
				};
				return View(viewModel);
			}
			catch (Exception e)
			{
				this.logger.LogError(e.Message);
				return this.RedirectToAction(nameof(Index), "Home");
			}
		}

		[HttpGet]
		public async Task<IActionResult> ManageUserRoles(string id)
		{
			try
			{
				var viewModel = await this.adminService.GetUserRolesAndShelterAsync(id);
				if (viewModel == null)
				{
					return NotFound();
				}
				return View(viewModel);
			}
			catch (Exception e)
			{
				this.logger.LogError(e.Message);
				return this.RedirectToAction(nameof(Index), "Home");
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ManageUserRoles(ManageUserRolesViewModel model)
		{
			try
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
			catch (Exception e)
			{
				this.logger.LogError(e.Message);
				return this.RedirectToAction(nameof(Index), "Home");
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteUser(string id)
		{
			try
			{
				var success = await this.adminService.DeleteUserAsync(id);

				if (success)
				{
					TempData["SuccessMessage"] = "User deleted successfully.";
				}
				else
				{
					TempData["ErrorMessage"] = "Failed to delete user.";
				}

				return RedirectToAction(nameof(AllUsers));
			}
			catch (Exception e)
			{
				this.logger.LogError(e.Message);
				return this.RedirectToAction(nameof(Index), "Home");
			}
		}
	}
}