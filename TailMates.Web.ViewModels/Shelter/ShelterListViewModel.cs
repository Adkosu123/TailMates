using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TailMates.Data.Models;

namespace TailMates.Web.ViewModels.Shelter
{
   public class ShelterListViewModel
    {
		public PaginatedList<ShelterViewModel> Shelters { get; set; }
	}
}
