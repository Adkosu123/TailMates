using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TailMates.Web.ViewModels.Pet
{
    public class PetListViewModel
    {
		public virtual IEnumerable<PetViewModel> Pets { get; set; }
			= new List<PetViewModel>();
	}
}
