using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TailMates.Web.ViewModels.Shelter;

namespace TailMates.Services.Core.Interfaces
{
    public interface IShelterService
    {
		Task<IEnumerable<ShelterViewModel>> GetAllSheltersAsync();

		Task<bool> AddShelterAsync(ShelterCreateViewModel model);

		Task<ShelterDetailsViewModel?> GetShelterDetailsAsync(int id);
	}
}
