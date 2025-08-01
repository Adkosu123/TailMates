using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TailMates.Data.Models;

namespace TailMates.Web.ViewModels.Pet
{
    public class PetListViewModel
    {
		public PaginatedList<PetViewModel> Pets { get; set; }

		public PetFilterViewModel Filters { get; set; }
		    = new PetFilterViewModel();
	}
}
