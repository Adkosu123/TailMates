using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using TailMates.Services.Core.Interfaces;
using TailMates.Services.Core.Services;
using TailMates.Web.ViewModels.Pet;

namespace TailMates.Web.Controllers
{
    public class PetController : BaseController
    {
		private readonly IPetService petService;

		public PetController(IPetService petService)
		{
			this.petService = petService;
		}

		public async Task<IActionResult> All()
        {
			var petViewModels = await petService.GetAllPetsAsync();
			var viewModel = new PetListViewModel
			{
				Pets = petViewModels
			};

			return View(viewModel);
		}

		public async Task<IActionResult> Details(int id) 
		{
			var petDetails = await petService.GetPetDetailsAsync(id);

			if (petDetails == null)
			{
				return NotFound();
			}
			return View(petDetails);
		}

		[HttpGet]
		[Authorize(Roles = "Admin,Manager")]
		public async Task<IActionResult> List()
		{
			string? currentUserId = GetUserId();

			if (currentUserId.IsNullOrEmpty())
			{
				return Unauthorized();
			}
			var isManager = User.IsInRole("Manager");
			var isAdmin = User.IsInRole("Admin");

			var dropdowns = await petService.GetPetFormDropdownsAsync(currentUserId, isManager);

			ViewBag.SpeciesList = dropdowns.SpeciesList;
			ViewBag.BreedList = dropdowns.BreedList;
			ViewBag.ShelterList = dropdowns.ShelterList;
			ViewBag.IsManagerOnly = isManager && !isAdmin;

			var model = new PetCreateViewModel();

			if (isManager && !isAdmin)
			{
				var assignedShelterItem = dropdowns.ShelterList?.FirstOrDefault(s => s.Selected);
				if (assignedShelterItem != null)
				{
					model.AssignedShelterDisplayText = assignedShelterItem.Text;
					model.AssignedShelterValue = assignedShelterItem.Value;
					model.ShelterId = int.Parse(assignedShelterItem.Value);
				}
			}

			return View(model);
		}

		[HttpPost]
		[Authorize(Roles = "Admin,Manager")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> List(PetCreateViewModel model)
		{
			var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var isAdmin = User.IsInRole("Admin");

			if (!isAdmin && model.ShelterId == 0 && currentUserId != null)
			{
				var dropdowns = await petService.GetPetFormDropdownsAsync(currentUserId, true);
				var assignedShelterItem = dropdowns.ShelterList?.FirstOrDefault(s => s.Selected);
				if (assignedShelterItem != null && int.TryParse(assignedShelterItem.Value, out int managerShelterId))
				{
					model.ShelterId = managerShelterId;
				}
				else
				{
					ModelState.AddModelError(string.Empty, "Manager's assigned shelter could not be determined.");
				}
			}

			if (ModelState.IsValid)
			{
				var success = await petService.AddPetAsync(model, currentUserId, isAdmin);
				if (success)
				{
					TempData["SuccessMessage"] = "Pet listed successfully!";
					return RedirectToAction(nameof(All));
				}
				else
				{
					TempData["ErrorMessage"] = "Failed to list pet. Please check your inputs.";
				}
			}

			var dropdownsOnErrors = await petService.GetPetFormDropdownsAsync(currentUserId, User.IsInRole("Manager"));
			ViewBag.SpeciesList = dropdownsOnErrors.SpeciesList;
			ViewBag.BreedList = dropdownsOnErrors.BreedList;
			ViewBag.ShelterList = dropdownsOnErrors.ShelterList;
			ViewBag.IsManagerOnly = User.IsInRole("Manager") && !isAdmin;

			TempData["ErrorMessage"] = TempData["ErrorMessage"] ?? "Please correct the errors in the form.";
			return View(model);
		}

		[HttpGet]
		public async Task<IActionResult> GetBreedsBySpecies(int speciesId)
		{
			var breeds = await petService.GetBreedsForSpeciesAsync(speciesId);
			return Json(breeds.Select(b => new { id = b.Id, name = b.Name }));
		}

	}
}
