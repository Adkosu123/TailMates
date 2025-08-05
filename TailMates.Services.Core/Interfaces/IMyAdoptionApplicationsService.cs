using TailMates.Data.Models;
using TailMates.Web.ViewModels.MyAdoptionApplications;

namespace TailMates.Services.Core.Interfaces
{
	public interface IMyAdoptionApplicationsService
	{
		Task<PaginatedList<AdoptionApplicationViewModel>> GetUserApplicationsAsync(string userId, int pageIndex, int pageSize);
	}
}
