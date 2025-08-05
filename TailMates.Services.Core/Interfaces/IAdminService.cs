using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TailMates.Data.Models;
using TailMates.Data.Models.Enums;
using TailMates.Web.ViewModels.Admin;

namespace TailMates.Services.Core.Interfaces
{
	public interface IAdminService
	{
		Task<PaginatedList<AdminAdoptionApplicationViewModel>> GetAllApplicationsAsync(int pageIndex, int pageSize);

		Task<AdoptionApplicationDetailsViewModel> GetApplicationDetailsAsync(int applicationId);

		Task<bool> UpdateApplicationStatusAndNotesAsync(int applicationId, ApplicationStatus newStatus, string adminNotes);

		Task<PaginatedList<UserViewModel>> GetAllUsersAsync(int pageIndex, int pageSize);
		Task<ManageUserRolesViewModel?> GetUserRolesAndShelterAsync(string userId);
		Task<bool> UpdateUserRolesAndShelterAsync(string userId, List<string> selectedRoles, int? managedShelterId);


	}
}
