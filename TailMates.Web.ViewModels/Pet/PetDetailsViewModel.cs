using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TailMates.Web.ViewModels.Pet
{
	public class PetDetailsViewModel
	{

		public int Id { get; set; }

		[Display(Name = "Pet Name")]
		public string Name { get; set; } = string.Empty;

		[Display(Name = "Age")]
		public int Age { get; set; }

		public string Description { get; set; } = string.Empty;

		[Display(Name = "Image")]
		public string? ImageUrl { get; set; }

		public string Gender { get; set; } = string.Empty;

		[Display(Name = "Listed On")]
		[DataType(DataType.Date)]
		public DateTime DateListed { get; set; }

		[Display(Name = "Is Adopted?")]
		public bool IsAdopted { get; set; }

		[Display(Name = "Species")]
		public string SpeciesName { get; set; } = string.Empty;

		[Display(Name = "Breed")]
		public string BreedName { get; set; } = string.Empty;

		[Display(Name = "Shelter Name")]
		public string ShelterName { get; set; } = string.Empty;

		[Display(Name = "Shelter Address")]
		public string ShelterAddress { get; set; } = string.Empty;

		[Display(Name = "Shelter Phone")]
		public string? ShelterPhoneNumber { get; set; }

		[Display(Name = "Shelter Email")]
		public string? ShelterEmail { get; set; }

		public int ShelterId { get; set; }

		public ICollection<AdoptionApplicationViewModel> AdoptionApplications { get; set; }
			  = new HashSet<AdoptionApplicationViewModel>();
	}
}
