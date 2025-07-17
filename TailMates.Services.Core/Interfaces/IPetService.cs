using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TailMates.Data.Models;
using TailMates.Web.ViewModels.Pet;

namespace TailMates.Services.Core.Interfaces
{
   public interface IPetService
    {
        Task<IEnumerable<PetViewModel>> GetAllPetsAsync();

        Task<PetDetailsViewModel?> GetPetDetailsAsync(int id);

		Task<bool> AddPetAsync(PetCreateViewModel newPetVm, string? currentUserId = null, bool isAdmin = false);


		Task<IEnumerable<Breed>> GetBreedsForSpeciesAsync(int speciesId);

		Task<PetFormDropdownsViewModel> GetPetFormDropdownsAsync(string? currentUserId = null, bool isManager = false);
	}
}
