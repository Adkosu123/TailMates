using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TailMates.Web.ViewModels.Pet;

namespace TailMates.Services.Core.Interfaces
{
   public interface IPetService
    {
        Task<IEnumerable<PetViewModel>> GetAllPetsAsync();

        Task<PetDetailsViewModel?> GetPetDetailsAsync(int id);

        // For Manager role - get pets specifically for their shelter, including unapproved ones
       // Task<IEnumerable<PetViewModel>> GetPetsForShelterManagerAsync(int shelterId);

        // For Manager role - approve a pet for public viewing
       // Task<bool> ApprovePetAsync(int petId);
       //
       // // For Manager role - create a pet (initial status will be unapproved)
       // Task<bool> CreatePetAsync(PetCreateEditViewModel model, int shelterId);
       //
       // // For Manager role - get a pet for editing
       // Task<PetCreateEditViewModel?> GetPetForEditAsync(int petId, int managerShelterId);
       //
       // // For Manager role - edit a pet
       // Task<bool> EditPetAsync(PetCreateEditViewModel model, int managerShelterId); // Added managerShelterId for authorization
       //
       // // For Manager role - delete a pet (soft delete)
       // Task<bool> DeletePetAsync(int petId, int managerShelterId); // Added managerShelterId for authorization
       //
       // // For Admin role - get all pets, including deleted and unapproved ones
       // Task<IEnumerable<PetViewModel>> GetAllPetsForAdminAsync();
       //
       // // For Admin role - get pet details for admin (can view deleted/unapproved)
       // Task<PetDetailsViewModel?> GetPetDetailsForAdminAsync(int id);
       //
       // // NEW: Methods to get data for dropdowns in Create/Edit forms
       // Task<IEnumerable<SelectListItem>> GetSpeciesAsSelectListItemsAsync();
       // Task<IEnumerable<SelectListItem>> GetBreedsAsSelectListItemsAsync(int speciesId);
       // IEnumerable<SelectListItem> GetGendersAsSelectListItems();
	}
}
