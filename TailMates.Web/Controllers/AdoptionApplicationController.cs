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
				var viewModel = await this.adoptionApplicationService.GetAdoptionApplicationViewModelAsync(petId);

				if (viewModel == null)
				{
					return this.RedirectToAction("All", "Pet");
				}

				return this.View(viewModel);
			}
			catch (Exception e)
			{
				logger.LogError(e, "An error occurred while preparing the adoption application form for pet ID {PetId}", petId);
				TempData["ErrorMessage"] = "An unexpected error occurred. Please try again later.";
				return this.RedirectToAction("All", "Pet");
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(AdoptionApplicationCreateViewModel viewModel)
		{
			try
			{
				if (!this.ModelState.IsValid)
				{
					var pet = await this.petService.GetPetDetailsForUserAsync(viewModel.PetId);
					if (pet != null)
					{
						viewModel.PetName = pet.Name;
						viewModel.PetImageUrl = pet.ImageUrl;
						viewModel.PetSpecies = pet.SpeciesName;
						viewModel.PetBreed = pet.BreedName;
						viewModel.PetAge = pet.Age;
						viewModel.PetDescription = pet.Description;
					}

					return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList() });
				}

				var userId = this.userManager.GetUserId(User);

				if (string.IsNullOrEmpty(userId))
				{
					return Json(new { success = false, errors = new List<string> { "You must be logged in to submit an application." } });
				}

				var success = await this.adoptionApplicationService.CreateAdoptionApplicationAsync(viewModel, userId);

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
				logger.LogError(e, "An error occurred while creating an adoption application for pet ID {PetId}", viewModel.PetId);
				return Json(new { success = false, errors = new List<string> { "An unexpected error occurred. Please try again." } });
			}
		}
	}
}