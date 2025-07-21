using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using TailMates.Data.Models;
using TailMates.Services.Core.Interfaces;
using TailMates.Services.Core.Services;
using TailMates.Web.ViewModels.Pet;

namespace TailMates.Web.Controllers
{
    public class PetController : BaseController
    {
		private readonly IPetService petService;
		private readonly UserManager<ApplicationUser> userManager;

		public PetController(IPetService petService,
			UserManager<ApplicationUser> userManager)
		{
			this.petService = petService;
			this.userManager = userManager;
		}

		[HttpGet]
		public async Task<IActionResult> All()
        {
			var petViewModels = await petService.GetAllPetsAsync();

			if (User.Identity.IsAuthenticated)
			{
				var user = await userManager.GetUserAsync(User);
				if (user != null && user.ManagedShelterId.HasValue)
				{
					ViewBag.UserManagedShelterId = user.ManagedShelterId.Value;
				}
			}

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

		[HttpGet]
		[Authorize(Roles = "Admin,Manager")]
		public async Task<IActionResult> Manage(int id)
		{
			var pet = await petService.GetPetDetailsForEditAsync(id);

			if (pet == null)
			{
				TempData["ErrorMessage"] = "Pet not found.";
				return RedirectToAction("All"); 
			}

			if (User.IsInRole("Manager")&& !User.IsInRole("Admin")) 
			{
				var currentUser = await userManager.GetUserAsync(User);
				if (currentUser == null || !currentUser.ManagedShelterId.HasValue || currentUser.ManagedShelterId.Value != pet.ShelterId)
				{
					TempData["ErrorMessage"] = "You are not authorized to edit pets outside your assigned shelter.";
					return RedirectToAction("Details", new { id = pet.Id });
				}
			}

			var editVm = new PetEditViewModel
			{
				Id = pet.Id,
				Name = pet.Name,
				Age = pet.Age,
				Description = pet.Description,
				ImageUrl = pet.ImageUrl,
				Gender = pet.Gender,
				SpeciesId = pet.SpeciesId,
				BreedId = pet.BreedId,
				ShelterId = pet.ShelterId, 
			};

			ViewBag.SpeciesList = await petService.GetSpeciesAsSelectListAsync();
			ViewBag.ShelterList = await petService.GetSheltersAsSelectListAsync();

			ViewBag.IsManagerOnly = User.IsInRole("Manager") && !User.IsInRole("Admin");

			if ((bool)ViewBag.IsManagerOnly)
			{
				var currentUser = await userManager.GetUserAsync(User);
				if (currentUser.ManagedShelterId.HasValue)
				{
					var assignedShelter = await petService.GetShelterByIdAsync(currentUser.ManagedShelterId.Value);
					if (assignedShelter != null)
					{
						editVm.AssignedShelterDisplayText = assignedShelter.Name;
						editVm.AssignedShelterValue = assignedShelter.Id;
					}
				}
			}

			ViewBag.BreedList = new SelectList(await petService.GetBreedsForSpeciesAsync(pet.SpeciesId), "Id", "Name", pet.BreedId);

			return View(editVm);
		}

		[HttpPost]
		[ValidateAntiForgeryToken] 
		[Authorize(Roles = "Admin,Manager")] 
		public async Task<IActionResult> Manage(PetEditViewModel model)
		{
			if (ModelState.IsValid)
			{
				if (User.IsInRole("Manager") && !User.IsInRole("Admin"))
				{
					var currentUser = await userManager.GetUserAsync(User);
					var originalPet = await petService.GetPetDetailsForEditAsync(model.Id); // Get original pet's shelter

					if (currentUser == null || !currentUser.ManagedShelterId.HasValue ||
						originalPet == null || originalPet.ShelterId != currentUser.ManagedShelterId.Value ||
						model.ShelterId != currentUser.ManagedShelterId.Value)
					{
						TempData["Message"] = "You are not authorized to modify this pet or change its assigned shelter.";
						TempData["MessageType"] = "error";
						return RedirectToAction("All", "Pet");
					}
				}

				bool success = await petService.UpdatePetAsync(model);

				if (success)
				{
					TempData["Message"] = $"Pet '{model.Name}' updated successfully!";
					TempData["MessageType"] = "success";
				}
				else
				{
					TempData["Message"] = $"Failed to update pet '{model.Name}'. Please try again.";
					TempData["MessageType"] = "error";
				}
			}
			else 
			{
				var errors = ModelState.Values.SelectMany(v => v.Errors)
											 .Select(e => e.ErrorMessage)
											 .ToList();
				TempData["Message"] = "Validation failed. " + string.Join(" ", errors);
				TempData["MessageType"] = "error";
			}

			return RedirectToAction("All", "Pet");

			ViewBag.SpeciesList = await petService.GetSpeciesAsSelectListAsync();
			ViewBag.ShelterList = await petService.GetSheltersAsSelectListAsync();
			ViewBag.IsManagerOnly = User.IsInRole("Manager") && !User.IsInRole("Admin");

			if ((bool)ViewBag.IsManagerOnly)
			{
				var currentUser = await userManager.GetUserAsync(User);
				if (currentUser.ManagedShelterId.HasValue)
				{
					var assignedShelter = await petService.GetShelterByIdAsync(currentUser.ManagedShelterId.Value);
					if (assignedShelter != null)
					{
						model.AssignedShelterDisplayText = assignedShelter.Name;
						model.AssignedShelterValue = assignedShelter.Id;
					}
				}
			}
			ViewBag.BreedList = new SelectList(await petService.GetBreedsForSpeciesAsync(model.SpeciesId), "Id", "Name", model.BreedId);


			return View(model);
		}
	}
}
