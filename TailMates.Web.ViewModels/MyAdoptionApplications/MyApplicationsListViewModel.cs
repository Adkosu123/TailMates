using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TailMates.Data.Models;

namespace TailMates.Web.ViewModels.MyAdoptionApplications
{
	public class MyApplicationsListViewModel
	{
		public PaginatedList<AdoptionApplicationViewModel> Applications { get; set; } = new PaginatedList<AdoptionApplicationViewModel>(new List<AdoptionApplicationViewModel>(), 0, 1, 10);
	}
}
