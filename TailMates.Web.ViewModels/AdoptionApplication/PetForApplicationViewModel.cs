using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TailMates.Web.ViewModels.AdoptionApplication
{
    public class PetForApplicationViewModel
    {
		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public string ImageUrl { get; set; } = "https://placehold.co/600x400/CCCCCC/000000?text=No+Image";
		public string SpeciesName { get; set; } = string.Empty;
		public string BreedName { get; set; } = string.Empty;
		public int Age { get; set; }
	}
}
