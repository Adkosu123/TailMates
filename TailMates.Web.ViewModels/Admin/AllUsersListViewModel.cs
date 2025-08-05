using TailMates.Data.Models;

namespace TailMates.Web.ViewModels.Admin
{
	public class AllUsersListViewModel
	{
		public PaginatedList<UserViewModel> Users { get; set; } = new PaginatedList<UserViewModel>(new List<UserViewModel>(), 0, 1, 10);
	}
}
