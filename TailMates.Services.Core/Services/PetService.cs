using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TailMates.Data.Models;
using TailMates.Data.Models.Enums;
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
				int changesSaved = await petRepository.SaveChangesAsync(); 

				if (changesSaved == 0)
				{
					return false;
				}

				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error adding pet: {ex.Message}");
				return false;
			}
		}

		public async Task<PaginatedList<PetViewModel>> GetFilteredPetsAsync(PetFilterViewModel filters, int pageIndex, int pageSize)
		{
			var petsQuery = petRepository
			   .AllAsNoTracking()
			   .Include(p => p.Breed)
			   .Include(p => p.Species)
			   .Include(p => p.Shelter)
			   .Where(p => !p.IsDeleted && !p.IsAdopted);

			if (!string.IsNullOrWhiteSpace(filters.SearchTerm))
			{
				var searchTermLower = filters.SearchTerm.ToLower();
				petsQuery = petsQuery.Where(p =>
					p.Name.ToLower().Contains(searchTermLower) ||
					(p.Description != null && p.Description.ToLower().Contains(searchTermLower)) ||
					p.Species.Name.ToLower().Contains(searchTermLower) ||
					p.Breed.Name.ToLower().Contains(searchTermLower) ||
					p.Shelter.Name.ToLower().Contains(searchTermLower)
				);
			}

			if (filters.SpeciesId.HasValue && filters.SpeciesId.Value > 0)
			{
				petsQuery = petsQuery.Where(p => p.SpeciesId == filters.SpeciesId.Value);
			}

			if (filters.BreedId.HasValue && filters.BreedId.Value > 0)
			{
				petsQuery = petsQuery.Where(p => p.BreedId == filters.BreedId.Value);
			}

			if (!string.IsNullOrWhiteSpace(filters.Gender))
			{
				if (Enum.TryParse<PetGender>(filters.Gender, true, out var genderEnum))
				{
					petsQuery = petsQuery.Where(p => p.Gender == genderEnum);
				}
			}

			if (filters.MinAge.HasValue)
			{
				petsQuery = petsQuery.Where(p => p.Age >= filters.MinAge.Value);
			}
			if (filters.MaxAge.HasValue)
			{
				petsQuery = petsQuery.Where(p => p.Age <= filters.MaxAge.Value);
			}

			if (filters.ShelterId.HasValue && filters.ShelterId.Value > 0)
			{
				petsQuery = petsQuery.Where(p => p.ShelterId == filters.ShelterId.Value);
			}

			var orderedPetsQuery = petsQuery.OrderBy(p => p.Name);

			var pagedPets = await PaginatedList<Pet>.CreateAsync(orderedPetsQuery, pageIndex, pageSize);

			var petViewModels = pagedPets.Select(p => new PetViewModel
			{
				Id = p.Id,
				Name = p.Name,
				Age = p.Age,
				Description = p.Description,
				ImageUrl = p.ImageUrl,
				Gender = p.Gender.ToString(),
				SpeciesName = p.Species.Name,
				BreedName = p.Breed.Name,
				ShelterId = p.ShelterId,
				ShelterName = p.Shelter.Name,
			}).ToList();

			return new PaginatedList<PetViewModel>(petViewModels, pagedPets.TotalCount, pagedPets.PageIndex, pagedPets.PageSize);
		}

		public async Task<IEnumerable<PetViewModel>> GetAllPetsAsync() // Keeping your original GetAllPetsAsync if it's used elsewhere without filters
		{
			var pets = await petRepository.GetAllPetsWithDetails().ToListAsync(); // Using the new IQueryable method
			return pets.Select(pet => new PetViewModel
			{
				Id = pet.Id,
				Name = pet.Name,
				Age = pet.Age,
				Description = pet.Description,
				ImageUrl = pet.ImageUrl,
				Gender = pet.Gender.ToString(),
				SpeciesName = pet.Species?.Name ?? "N/A",
				BreedName = pet.Breed?.Name ?? "N/A",
				ShelterName = pet.Shelter?.Name ?? "N/A",
				ShelterId = pet.ShelterId
			}).ToList();
		}

		public async Task<IEnumerable<Breed>> GetBreedsForSpeciesAsync(int speciesId) // NEW implementation
		{
			return await this.breedRepository.GetBySpeciesIdAsync(speciesId);
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

		public async Task<SelectList> GetSpeciesAsSelectListAsync(int? selectedId = null)
		{
			var species = await petRepository.GetAllSpeciesLookupAsync(); 
			var items = species.Select(s => new SelectListItem
			{
				Value = s.Id.ToString(),
				Text = s.Name
			}).OrderBy(li => li.Text)
			  .Prepend(new SelectListItem { Value = "", Text = "All Species", Selected = selectedId == null || selectedId == 0 })
			  .ToList();

			return new SelectList(items, "Value", "Text", selectedId);
		}

		public async Task<SelectList> GetSheltersAsSelectListAsync(int? selectedId = null)
		{
			var shelters = await petRepository.GetAllSheltersLookupAsync(); 
			var items = shelters.Select(s => new SelectListItem
			{
				Value = s.Id.ToString(),
				Text = s.Name
			}).OrderBy(li => li.Text)
			  .Prepend(new SelectListItem { Value = "", Text = "All Shelters", Selected = selectedId == null || selectedId == 0 })
			  .ToList();

			return new SelectList(items, "Value", "Text", selectedId);
		}

		public Task<IEnumerable<SelectListItem>> GetGendersSelectListAsync(string? selectedGender = null)
		{
			var genderOptions = Enum.GetValues(typeof(PetGender)) 
									.Cast<PetGender>()
									.Select(g => new SelectListItem
									{
										Value = g.ToString(),
										Text = g.ToString(),
										Selected = g.ToString().Equals(selectedGender, StringComparison.OrdinalIgnoreCase)
									})
									.OrderBy(li => li.Text)
									.Prepend(new SelectListItem { Value = "", Text = "All Genders", Selected = string.IsNullOrEmpty(selectedGender) })
									.ToList();
			return Task.FromResult<IEnumerable<SelectListItem>>(genderOptions);
		}

		public async Task<IEnumerable<SelectListItem>> GetBreedsSelectListForSpeciesAsync(int speciesId, int? selectedId = null)
		{
			if (speciesId <= 0)
			{
				return new List<SelectListItem>
				{
					new SelectListItem { Value = "", Text = "All Breeds", Selected = selectedId == null || selectedId == 0 }
				};
			}

			var breeds = await petRepository.GetBreedsForSpeciesLookupAsync(speciesId);
			return breeds.Select(b => new SelectListItem
			{
				Value = b.Id.ToString(),
				Text = b.Name,
				Selected = b.Id == selectedId
			}).OrderBy(li => li.Text)
			  .Prepend(new SelectListItem { Value = "", Text = "All Breeds", Selected = selectedId == null || selectedId == 0 })
			  .ToList();
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

		public async Task<bool> RemovePetAsync(int petId)
		{
			var pet = await this.petRepository.GetByIdAsync(petId);
			if (pet == null)
			{
				return false;
			}

			pet.IsDeleted = true;
			this.petRepository.Update(pet);
			await this.petRepository.SaveChangesAsync();

			return true;
		}

		public async Task<int?> GetPetShelterIdAsync(int petId)
		{
			var pet = await this.petRepository.GetByIdAsync(petId);
			return pet?.ShelterId;
		}

		public async Task<Pet> GetPetByIdWithAdoptionDetailsAsync(int petId)
		{
			return await this.petRepository.AllAsNoTracking()
			   .IgnoreQueryFilters()
			   .Include(p => p.AdoptionApplications)
			   .Include(p => p.Species) 
			   .Include(p => p.Breed) 
			   .Include(p => p.Shelter) 
			   .FirstOrDefaultAsync(p => p.Id == petId);
		}

		public async Task<PetDetailsViewModel?> GetPetDetailsForUserAsync(int petId)
		{
			var pet = await this.petRepository.AllAsNoTracking()
				.IgnoreQueryFilters()
				.Include(p => p.AdoptionApplications)
				.Include(p => p.Species)
				.Include(p => p.Breed)
				.Include(p => p.Shelter)
				.FirstOrDefaultAsync(p => p.Id == petId);

			if (pet == null)
			{
				return null;
			}

			return new PetDetailsViewModel
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
				ShelterId = pet.ShelterId
			};
		}
	}
}
