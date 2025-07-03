using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TailMates.Data.Models;
using TailMates.Services.Core.Interfaces;
using TailMates.Services.Core.Services;
using TailMates.Web.ViewModels.AdoptionApplication;

namespace TailMates.Web.Controllers
{
	public class AdoptionApplication : BaseController
	{
		private readonly IPetService petService;
		private readonly IAdoptionApplicationService adoptionApplicationService;
		private readonly UserManager<ApplicationUser> userManager;

		public AdoptionApplication(IPetService petService,
			IAdoptionApplicationService adoptionApplicationService,
			UserManager<ApplicationUser> userManager)
		{
			this.petService = petService;
			this.adoptionApplicationService = adoptionApplicationService;
			this.userManager = userManager;
		}

		[HttpGet]
		public async Task<IActionResult> Create(int petId)
		{
			var pet = await petService.GetPetDetailsAsync(petId);

			// If the pet is not found or is already adopted, redirect the user.
			if (pet == null || pet.IsAdopted)
			{
				TempData["ErrorMessage"] = "The pet you are trying to apply for is not available or does not exist.";
				return RedirectToAction("All", "Pet"); // Redirect to all pets page
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

		[HttpPost]
		[ValidateAntiForgeryToken] // Protects against Cross-Site Request Forgery attacks
		public async Task<IActionResult> Create(AdoptionApplicationCreateViewModel model)
		{
			// If the model state is not valid, return JSON indicating failure with errors.
			if (!ModelState.IsValid)
			{
				// Re-fetch pet details to re-populate the read-only pet info on the form
				// This is important so pet details don't disappear if validation fails.
				var pet = await petService.GetPetDetailsAsync(model.PetId);
				if (pet != null)
				{
					model.PetName = pet.Name;
					model.PetImageUrl = pet.ImageUrl;
					model.PetSpecies = pet.SpeciesName;
					model.PetBreed = pet.BreedName;
					model.PetAge = pet.Age;
				}
				// Return a JSON response with validation errors
				// SelectMany is used to flatten all error messages from ModelState
				return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList() });
			}

			// Get the ID of the currently logged-in user using the UserManager.
			// This relies on the user being authenticated (enforced by [Authorize] attribute on the controller).
			var userId = userManager.GetUserId(User);

			// Although [Authorize] should prevent this, it's a good safeguard.
			if (string.IsNullOrEmpty(userId))
			{
				return Json(new { success = false, errors = new List<string> { "You must be logged in to submit an application." } });
			}

			// Call the service to create the adoption application.
			var success = await adoptionApplicationService.CreateApplicationAsync(model, userId);

			if (success)
			{
				// On successful application, return a JSON success response.
				return Json(new { success = true, message = "Your adoption application has been submitted successfully!" });
			}
			else
			{
				// On failure (e.g., pet not available, duplicate application), return JSON failure response.
				return Json(new { success = false, errors = new List<string> { "Could not submit application. The pet may no longer be available, or you may have already applied for this pet." } });
			}
		}
	}
}
