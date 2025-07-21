using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TailMates.Web.ViewModels.Pet
{
	public class PetViewModel
	{
		public int Id { get; set; }

		[Display(Name = "Pet Name")]
		public string Name { get; set; } = string.Empty;

		[Display(Name = "Age (Years)")]
		public int Age { get; set; }

		public string Description { get; set; } = string.Empty;

		[Display(Name = "Image")]
		public string? ImageUrl { get; set; }

		public string Gender { get; set; } = string.Empty; 

		[Display(Name = "Species Type")]
		public string SpeciesName { get; set; } = string.Empty;

		[Display(Name = "Breed Type")]
		public string BreedName { get; set; } = string.Empty;

		[Display(Name = "Shelter Location")]
		public string ShelterName { get; set; } = string.Empty;

		public int ShelterId { get; set; }
	}
}
