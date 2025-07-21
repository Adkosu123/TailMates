using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TailMates.Data;
using TailMates.Data.Models;
using TailMates.Data.Models.Enums;
using TailMates.Data.Repositories.Implementations;
using TailMates.Data.Repositories.Interfaces;
using TailMates.Services.Core.Interfaces;
using TailMates.Web.ViewModels.Pet;

namespace TailMates.Services.Core.Services
{
	public class PetService : IPetService
	{

		private readonly IPetRepository petRepository;
		private readonly ISpeciesRepository speciesRepository;
		private readonly IBreedRepository breedRepository;
		private readonly IShelterRepository shelterRepository;
		private readonly UserManager<ApplicationUser> userManager;

		public PetService(IPetRepository petRepository,
			ISpeciesRepository speciesRepository,
			IBreedRepository breedRepository,
			IShelterRepository shelterRepository,
			UserManager<ApplicationUser> userManager)
		{
			this.petRepository = petRepository;
			this.speciesRepository = speciesRepository;
			this.breedRepository = breedRepository;
			this.shelterRepository = shelterRepository;
			this.userManager = userManager;
		}

		public async Task<bool> AddPetAsync(PetCreateViewModel newPetVm, string? currentUserId = null, bool isAdmin = false)
		{
			if (!isAdmin && currentUserId != null)
			{
				var user = await userManager.FindByIdAsync(currentUserId);
				if (user == null || !user.ManagedShelterId.HasValue || user.ManagedShelterId.Value != newPetVm.ShelterId)
				{
					return false;
				}
			}

			var newPet = new Pet
			{
				Name = newPetVm.Name,
				Age = newPetVm.Age,
				Description = newPetVm.Description,
				ImageUrl = newPetVm.ImageUrl,
				Gender = newPetVm.Gender,
				SpeciesId = newPetVm.SpeciesId,
				BreedId = newPetVm.BreedId,
				ShelterId = newPetVm.ShelterId,
				DateListed = DateTime.UtcNow,
				IsAdopted = false
			};

			try
			{
				await petRepository.AddAsync(newPet);
				await petRepository.SaveChangesAsync();
				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error adding pet: {ex.Message}");
				return false;
			}
		}

		public async Task<IEnumerable<PetViewModel>> GetAllPetsAsync()
		{
			var pets = await petRepository.GetAllPetsWithDetailsAsync();
	
			var petViewModels = pets.Select(p => new PetViewModel
			{
				Id = p.Id,
				Name = p.Name,
				Age = p.Age,
				Description = p.Description,
				ImageUrl = p.ImageUrl,
				Gender = p.Gender.ToString(), 
				SpeciesName = p.Species?.Name ?? "N/A", 
				BreedName = p.Breed?.Name ?? "N/A",
				ShelterName = p.Shelter?.Name ?? "N/A",
				ShelterId = p.ShelterId,
			}).ToList();

			return petViewModels;
		}

		public async Task<IEnumerable<Breed>> GetBreedsForSpeciesAsync(int speciesId) // NEW implementation
		{
			return await breedRepository.GetBySpeciesIdAsync(speciesId);
		}

		public async Task<PetDetailsViewModel?> GetPetDetailsAsync(int id)
		{
			var pet = await petRepository.GetPetByIdWithDetailsAsync(id);

			if (pet == null)
			{
				return null; 
			}

			var petDetails = new PetDetailsViewModel
			{
				Id = pet.Id,
				Name = pet.Name,
				Age = pet.Age,
				Description = pet.Description,
				ImageUrl = pet.ImageUrl,
				Gender = pet.Gender.ToString(),
				DateListed = pet.DateListed,
				IsAdopted = pet.IsAdopted,
				SpeciesName = pet.Species?.Name ?? "N/A", 
				BreedName = pet.Breed?.Name ?? "N/A",
				ShelterName = pet.Shelter?.Name ?? "N/A",
				ShelterAddress = pet.Shelter?.Address ?? "N/A",
				ShelterPhoneNumber = pet.Shelter?.PhoneNumber,
				ShelterEmail = pet.Shelter?.Email,
				ShelterId = pet.ShelterId,

			};

			return petDetails;
		}

		public async Task<Pet> GetPetDetailsForEditAsync(int id)
		{
			return await petRepository.GetByIdAsync(id);
		}

		public async Task<PetFormDropdownsViewModel> GetPetFormDropdownsAsync(string? currentUserId = null, bool isManager = false)
		{
			var species = await speciesRepository.GetAllAsync();
			var breeds = await breedRepository.GetAllAsync();
			var allShelters = await shelterRepository.GetAllAsync();

			int? managerAssignedShelterId = null;
			if (isManager && currentUserId != null)
			{
				var managerUser = await userManager.FindByIdAsync(currentUserId);
				if (managerUser != null && managerUser.ManagedShelterId.HasValue)
				{
					managerAssignedShelterId = managerUser.ManagedShelterId.Value;
				}
			}

			var speciesList = new SelectList(species, "Id", "Name");
			var breedList = new SelectList(breeds, "Id", "Name");
			var statusList = new SelectList(Enum.GetNames(typeof(PetStatus)));

			SelectList shelterList;
			if (managerAssignedShelterId.HasValue)
			{
				shelterList = new SelectList(
					allShelters.Where(s => s.Id == managerAssignedShelterId.Value).ToList(),
					"Id",
					"Name",
					managerAssignedShelterId.Value
				);
			}
			else
			{
				shelterList = new SelectList(allShelters, "Id", "Name");
			}

			return new PetFormDropdownsViewModel
			{
				SpeciesList = speciesList,
				BreedList = breedList,
				ShelterList = shelterList,
				StatusList = statusList
			};
		}

		public async Task<Shelter> GetShelterByIdAsync(int id)
		{
			return await shelterRepository.GetByIdAsync(id);
		}

		public async Task<SelectList> GetSheltersAsSelectListAsync()
		{
			var shelters = await shelterRepository.GetAllAsync();
			return new SelectList(shelters, "Id", "Name");
		}

		public async Task<SelectList> GetSpeciesAsSelectListAsync()
		{
			var species = await speciesRepository.GetAllAsync();
			return new SelectList(species, "Id", "Name");
		}

		public async Task<bool> UpdatePetAsync(PetEditViewModel updatedPetVm)
		{
			var existingPet = await petRepository.GetByIdAsync(updatedPetVm.Id);

			if (existingPet == null)
			{
				return false; 
			}

			existingPet.Name = updatedPetVm.Name;
			existingPet.Age = updatedPetVm.Age;
			existingPet.Description = updatedPetVm.Description;
			existingPet.ImageUrl = updatedPetVm.ImageUrl;
			existingPet.Gender = updatedPetVm.Gender;
			existingPet.SpeciesId = updatedPetVm.SpeciesId;
			existingPet.BreedId = updatedPetVm.BreedId;
			existingPet.ShelterId = updatedPetVm.ShelterId; 

			try
			{
				await petRepository.SaveChangesAsync();
				return true;
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine($"Error updating pet with ID {updatedPetVm.Id}: {ex.Message}");
				Console.Error.WriteLine($"Stack Trace: {ex.StackTrace}");
				return false;
			}
		}
	}
}
