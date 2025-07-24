using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TailMates.Web.ViewModels.Pet
{
	public class PetFilterViewModel
	{
		[Display(Name = "Search Term")]
		public string? SearchTerm { get; set; }

		[Display(Name = "Species")]
		public int? SpeciesId { get; set; }
		public IEnumerable<SelectListItem> SpeciesOptions { get; set; } = new List<SelectListItem>();

		[Display(Name = "Breed")]
		public int? BreedId { get; set; }
		public IEnumerable<SelectListItem> BreedOptions { get; set; } = new List<SelectListItem>();

		[Display(Name = "Gender")]
		public string? Gender { get; set; } // Stored as string, convert to enum in service for filtering
		public IEnumerable<SelectListItem> GenderOptions { get; set; } = new List<SelectListItem>();

		[Display(Name = "Minimum Age")]
		[Range(0, 50, ErrorMessage = "Age must be between 0 and 50.")] // Example validation
		public int? MinAge { get; set; }

		[Display(Name = "Maximum Age")]
		[Range(0, 50, ErrorMessage = "Age must be between 0 and 50.")] // Example validation
		public int? MaxAge { get; set; }

		[Display(Name = "Shelter")]
		public int? ShelterId { get; set; }
		public IEnumerable<SelectListItem> ShelterOptions { get; set; } = new List<SelectListItem>();
	}
}
