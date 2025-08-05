using Microsoft.AspNetCore.Mvc.Rendering;
using TailMates.Data.Models;
using TailMates.Web.ViewModels.Pet;

namespace TailMates.Services.Core.Interfaces
{
   public interface IPetService
    {
        Task<IEnumerable<PetViewModel>> GetAllPetsAsync();

		Task<PaginatedList<PetViewModel>> GetFilteredPetsAsync(PetFilterViewModel filters, int pageIndex, int pageSize);

		Task<bool> AddPetAsync(PetCreateViewModel newPetVm, string? currentUserId = null, bool isAdmin = false);

	    Task<Pet> GetPetByIdWithAdoptionDetailsAsync(int petId);

		Task<PetDetailsViewModel?> GetPetDetailsForUserAsync(int petId);

		Task<IEnumerable<Breed>> GetBreedsForSpeciesAsync(int speciesId);

		Task<Pet> GetPetDetailsForEditAsync(int id);
		Task<bool> UpdatePetAsync(PetEditViewModel updatedPetVm);

		Task<bool> RemovePetAsync(int petId);

		Task<int?> GetPetShelterIdAsync(int petId);

		Task<SelectList> GetSpeciesAsSelectListAsync(int? selectedId = null);
		Task<SelectList> GetSheltersAsSelectListAsync(int? selectedId = null);
		Task<IEnumerable<SelectListItem>> GetGendersSelectListAsync(string? selectedGender = null);
		Task<IEnumerable<SelectListItem>> GetBreedsSelectListForSpeciesAsync(int speciesId, int? selectedId = null);
		Task<Shelter> GetShelterByIdAsync(int id);

		Task<PetFormDropdownsViewModel> GetPetFormDropdownsAsync(string? currentUserId = null, bool isManager = false);
	}
}
