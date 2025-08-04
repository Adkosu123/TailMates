using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using TailMates.Data.Models;
using TailMates.Data.Models.Enums;
using TailMates.Services.Core.Interfaces;
using TailMates.Services.Core.Services;
using TailMates.Web.ViewModels.Pet;

namespace TailMates.Web.Controllers
{
    public class PetController : BaseController
    {
		private readonly IPetService petService;
		private readonly UserManager<ApplicationUser> userManager;
		private readonly ILogger<PetController> logger;

		public PetController(IPetService petService,
			UserManager<ApplicationUser> userManager,
			ILogger<PetController> logger)
		{
			this.petService = petService;
			this.userManager = userManager;
			this.logger = logger;
		}

		[HttpGet]
		public async Task<IActionResult> All(PetFilterViewModel filters, int pageIndex = 1)
		{
			const int PageSize = 6;
			try
			{
				var paginatedPets = await petService.GetFilteredPetsAsync(filters, pageIndex, PageSize);
				filters.SpeciesOptions = await petService.GetSpeciesAsSelectListAsync(filters.SpeciesId);
				filters.GenderOptions = await petService.GetGendersSelectListAsync(filters.Gender);
				filters.ShelterOptions = await petService.GetSheltersAsSelectListAsync(filters.ShelterId);

				if (filters.SpeciesId.HasValue && filters.SpeciesId.Value > 0)
				{
					filters.BreedOptions = await petService.GetBreedsSelectListForSpeciesAsync(filters.SpeciesId.Value, filters.BreedId);
				}
				else
				{
					filters.BreedOptions = await petService.GetBreedsSelectListForSpeciesAsync(0, filters.BreedId);
				}

				var viewModel = new PetListViewModel
				{
					Pets = paginatedPets,
					Filters = filters
				};

				if (User.Identity != null && User.Identity.IsAuthenticated)
				{
					var user = await userManager.GetUserAsync(User);
					if (user != null && user.ManagedShelterId.HasValue)
					{
						ViewBag.UserManagedShelterId = user.ManagedShelterId.Value;
					}
				}

				if (TempData["Message"] != null)
				{
					ViewBag.Message = TempData["Message"];
					ViewBag.MessageType = TempData["MessageType"];
				}

				return View(viewModel);
			}
			catch (Exception e)
			{
				this.logger.LogError(e, "An error occurred while fetching pets.");
				return this.RedirectToAction(nameof(Index), "Home");
			}
		}

		[HttpGet]
		public async Task<IActionResult> Details(int id)
		{
			try
			{
				var petDetails = await this.petService.GetPetDetailsForUserAsync(id);

				if (petDetails == null)
				{
					return NotFound();
				}

				if (petDetails.IsAdopted)
				{
					var currentUser = await this.userManager.GetUserAsync(User);
					if (currentUser == null)
					{
						return NotFound();
					}

					var pet = await this.petService.GetPetByIdWithAdoptionDetailsAsync(id);
					var approvedApplication = pet?.AdoptionApplications
						.FirstOrDefault(a => a.ApplicantId == currentUser.Id && a.Status == ApplicationStatus.Approved);

					if (approvedApplication == null && !User.IsInRole("Admin") && !User.IsInRole("Manager"))
					{
						return NotFound();
					}
				}

				return View(petDetails);
			}
			catch (Exception e)
			{
				this.logger.LogError(e.Message);
				return this.RedirectToAction(nameof(Index));
			}
		}

		[HttpGet]
		[Authorize(Roles = "Admin,Manager")]
		public async Task<IActionResult> List()
		{
			try
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
			catch (Exception e)
			{
				this.logger.LogError(e.Message);
				return this.RedirectToAction(nameof(Index));
			}
		}

		[HttpPost]
		[Authorize(Roles = "Admin,Manager")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> List(PetCreateViewModel model)
		{
			try
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
						TempData["Message"] = "Pet listed successfully!";
						TempData["MessageType"] = "success";
						return RedirectToAction(nameof(All));
					}
					else
					{
						this.logger.LogError("Failed to list pet.");
						TempData["Message"] = "Failed to list pet. Please check your inputs.";
						TempData["MessageType"] = "error";
					}
				}

				var dropdownsOnErrors = await petService.GetPetFormDropdownsAsync(currentUserId, User.IsInRole("Manager"));
				ViewBag.SpeciesList = dropdownsOnErrors.SpeciesList;
				ViewBag.BreedList = dropdownsOnErrors.BreedList;
				ViewBag.ShelterList = dropdownsOnErrors.ShelterList;
				ViewBag.IsManagerOnly = User.IsInRole("Manager") && !isAdmin;

				TempData["Message"] = TempData["Message"] ?? "Please correct the errors in the form.";
				TempData["MessageType"] = TempData["MessageType"] ?? "error";
				return View(model);
			}
			catch (Exception e)
			{
				this.logger.LogError(e.Message);
				return this.RedirectToAction(nameof(Index));
			}
		}

		[HttpGet]
		public async Task<JsonResult> GetBreedsBySpecies(int speciesId)
		{
			var breeds = await petService.GetBreedsForSpeciesAsync(speciesId);
			return Json(breeds);
		}

		[HttpGet]
		[Authorize(Roles = "Admin,Manager")]
		public async Task<IActionResult> Manage(int id)
		{
			try
			{
				var pet = await petService.GetPetDetailsForEditAsync(id);

				if (pet == null)
				{
					TempData["ErrorMessage"] = "Pet not found.";
					return RedirectToAction("All");
				}

				if (User.IsInRole("Manager") && !User.IsInRole("Admin"))
				{
					var currentUser = await userManager.GetUserAsync(User);
					if (currentUser == null || !currentUser.ManagedShelterId.HasValue || currentUser.ManagedShelterId.Value != pet.ShelterId)
					{
						TempData["ErrorMessage"] = "You are not authorized to edit pets outside your assigned shelter.";
						return Unauthorized();
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
			catch (Exception e)
			{

				this.logger.LogError(e.Message);
				return this.RedirectToAction(nameof(Index));
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken] 
		[Authorize(Roles = "Admin,Manager")] 
		public async Task<IActionResult> Manage(PetEditViewModel model)
		{
			try
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
							return Unauthorized();
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
						this.logger.LogError("Failed to update pet!");
						TempData["Message"] = $"Failed to update pet '{model.Name}'. Please try again.";
						TempData["MessageType"] = "error";
					}
				}
				else
				{
					this.logger.LogError("Failed to update pet!");
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
			catch (Exception e)
			{

				this.logger.LogError(e.Message);
				return this.RedirectToAction(nameof(Index));
			}
		}
		[HttpPost]
		[Authorize(Roles = "Admin,Manager")]
		public async Task<IActionResult> Delete(int id)
		{
			try
			{
				if (User.IsInRole("Manager"))
				{
					var petShelterId = await petService.GetPetShelterIdAsync(id);
					var user = await userManager.GetUserAsync(User);
					var managerShelterId = user?.ManagedShelterId;

					if (petShelterId == null || petShelterId != managerShelterId)
					{
						TempData["Message"] = "You do not have permission to remove this pet.";
						TempData["MessageType"] = "error";
						return RedirectToAction("All");
					}
				}

				bool success = await petService.RemovePetAsync(id);
				if (success)
				{
					TempData["Message"] = "Pet removed successfully!";
					TempData["MessageType"] = "success";
				}
				else
				{
					TempData["Message"] = "Failed to remove pet. Please try again.";
					TempData["MessageType"] = "error";
				}

				return RedirectToAction("All");
			}
			catch (Exception e)
			{
				this.logger.LogError(e.Message);
				return this.RedirectToAction(nameof(Index));
			}
		}
	}
}
