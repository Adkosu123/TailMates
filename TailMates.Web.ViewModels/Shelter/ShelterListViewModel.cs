using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TailMates.Web.ViewModels.Shelter
{
   public class ShelterListViewModel
    {
		public IEnumerable<ShelterViewModel> Shelters { get; set; }
			= new List<ShelterViewModel>();
	}
}
