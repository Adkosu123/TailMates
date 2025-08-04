using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TailMates.Data.Models;
using TailMates.Web.ViewModels.Admin;

namespace TailMates.Services.Core.Interfaces
{
	public interface IAdminService
	{
		Task<PaginatedList<AdminAdoptionApplicationViewModel>> GetAllApplicationsAsync(int pageIndex, int pageSize);

		Task<AdoptionApplicationDetailsViewModel> GetApplicationDetailsAsync(int applicationId);
	}
}
