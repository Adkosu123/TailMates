using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TailMates.Data.Models;
using TailMates.Web.ViewModels.Shelter;

namespace TailMates.Services.Core.Interfaces
{
    public interface IShelterService
    {
		Task<PaginatedList<ShelterViewModel>> GetAllSheltersAsync(int pageIndex, int pageSize);

		Task<bool> AddShelterAsync(ShelterCreateViewModel model);

		Task<ShelterDetailsViewModel> GetShelterDetailsWithPaginatedPetsAsync(int shelterId, int pageIndex, int pageSize);

		Task<ShelterEditViewModel?> GetShelterForEditAsync(int id);
		Task<bool> UpdateShelterAsync(ShelterEditViewModel model);

	}
}
