using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TailMates.Data.Models;
using TailMates.Services.Core.Interfaces;
using TailMates.Services.Core.Services;
using TailMates.Web.ViewModels.AdoptionApplication;

namespace TailMates.Web.Controllers
{
	public class AdoptionApplicationController : BaseController
	{
		private readonly IPetService petService;
		private readonly IAdoptionApplicationService adoptionApplicationService;
		private readonly UserManager<ApplicationUser> userManager;
		private readonly ILogger<AdoptionApplicationController> logger;

		public AdoptionApplicationController(IPetService petService,
			IAdoptionApplicationService adoptionApplicationService,
			UserManager<ApplicationUser> userManager,
			ILogger<AdoptionApplicationController> logger)
		{
			this.petService = petService;
			this.adoptionApplicationService = adoptionApplicationService;
			this.userManager = userManager;
			this.logger = logger;
		}

		[HttpGet]
		public async Task<IActionResult> Create(int petId)
		{
			try
			{
				var pet = await petService.GetPetDetailsAsync(petId);

				// If the pet is not found or is already adopted, redirect the user.
				if (pet == null || pet.IsAdopted)
				{
					TempData["ErrorMessage"] = "The pet you are trying to apply for is not available or does not exist.";
					return RedirectToAction("All", "Pet");
				}

				// Create the ViewModel to populate the form with pet details.
				var viewModel = new AdoptionApplicationCreateViewModel
				{
					PetId = pet.Id,
					PetName = pet.Name,
					PetImageUrl = pet.ImageUrl,
					PetSpecies = pet.SpeciesName,
					PetBreed = pet.BreedName,
					PetAge = pet.Age
				};

				return View(viewModel);

			}
			catch (Exception e)
			{
				this.logger.LogError(e.Message);
				return this.RedirectToAction("All", "Pet");
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken] // Protects against Cross-Site Request Forgery attacks
		public async Task<IActionResult> Create(AdoptionApplicationCreateViewModel model)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					var pet = await petService.GetPetDetailsAsync(model.PetId);
					if (pet != null)
					{
						model.PetName = pet.Name;
						model.PetImageUrl = pet.ImageUrl;
						model.PetSpecies = pet.SpeciesName;
						model.PetBreed = pet.BreedName;
						model.PetAge = pet.Age;
					}
					return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList() });
				}

				var userId = userManager.GetUserId(User);

				if (string.IsNullOrEmpty(userId))
				{
					return Json(new { success = false, errors = new List<string> { "You must be logged in to submit an application." } });
				}

				var success = await adoptionApplicationService.CreateApplicationAsync(model, userId);

				if (success)
				{
					return Json(new { success = true, message = "Your adoption application has been submitted successfully!" });
				}
				else
				{
					return Json(new { success = false, errors = new List<string> { "Could not submit application. The pet may no longer be available, or you may have already applied for this pet." } });
				}
			}

			catch (Exception e)
			{
				this.logger.LogError(e.Message);
				return this.RedirectToAction("All", "Pets");
			}
		}
	}
}