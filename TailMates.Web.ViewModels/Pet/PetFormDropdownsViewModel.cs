using Microsoft.AspNetCore.Mvc.Rendering;

namespace TailMates.Web.ViewModels.Pet
{
	public class PetFormDropdownsViewModel
	{
		public SelectList SpeciesList { get; set; } = null!;
		public SelectList BreedList { get; set; } = null!;
		public SelectList ShelterList { get; set; } = null!;
		public SelectList StatusList { get; set; } = null!;
	}
}
