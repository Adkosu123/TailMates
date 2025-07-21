using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TailMates.Data.Models.Enums;
using static TailMates.Data.Common.ValidationConstants.Pet;

namespace TailMates.Web.ViewModels.Pet
{
	public class PetEditViewModel
	{
		public int Id { get; set; }

		[Required(ErrorMessage = "Pet name is required.")]
		[MaxLength(PetNameMaxLength, ErrorMessage = "Name cannot exceed 100 characters.")]
		[MinLength(PetNameMinLength, ErrorMessage = "Name must be atleast 2 characters.")]
		[Display(Name = "Pet Name")]
		public string Name { get; set; } = null!;

		[Required(ErrorMessage = "Age is required.")]
		[Range(PetAgeMinValue, PetAgeMaxValue, ErrorMessage = "Age must be between 1 and 30 years.")]
		public int Age { get; set; }

		[Required(ErrorMessage = "Description is required.")]
		[MaxLength(PetDescriptionMaxLength, ErrorMessage = "Description cannot exceed 500 characters.")]
		[MinLength(PetDescriptionMinLength, ErrorMessage = "Description must be atleast 10 characters.")]
		[Display(Name = "Description")]
		public string Description { get; set; } = null!;

		[Required(ErrorMessage = "Image URL is required.")]
		[Url(ErrorMessage = "Please enter a valid URL for the image.")]
		[Display(Name = "Image URL")]
		public string ImageUrl { get; set; } = null!;

		[Required(ErrorMessage = "Gender is required.")]
		[Display(Name = "Gender")]
		public PetGender Gender { get; set; } = PetGender.Unspecified;

		[Required(ErrorMessage = "Species is required.")]
		[Display(Name = "Species")]
		public int SpeciesId { get; set; }

		[Required(ErrorMessage = "Breed is required.")]
		[Display(Name = "Breed")]
		public int BreedId { get; set; }

		[Required(ErrorMessage = "Shelter is required.")]
		[Display(Name = "Shelter")]
		public int ShelterId { get; set; }
		public string? AssignedShelterDisplayText { get; set; }
		public int AssignedShelterValue { get; set; }
	}
}
