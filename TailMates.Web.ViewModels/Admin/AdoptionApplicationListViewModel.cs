using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TailMates.Data.Models;

namespace TailMates.Web.ViewModels.Admin
{
	public class AdoptionApplicationListViewModel
	{
		public PaginatedList<AdminAdoptionApplicationViewModel> Applications { get; set; } = new PaginatedList<AdminAdoptionApplicationViewModel>(new List<AdminAdoptionApplicationViewModel>(), 0, 1, 10);
	}
}
