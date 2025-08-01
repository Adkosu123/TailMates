using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TailMates.Web.ViewModels.AdoptionApplication
{
    public class AdoptionApplicationCreateViewModel
    {
		[Required(ErrorMessage = "Pet ID is required.")]
		public int PetId { get; set; }

		[Display(Name = "Your Notes for the Shelter")]
		[StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters.")]
		public string? ApplicantNotes { get; set; }

		public string PetName { get; set; } = string.Empty;
		public string? PetImageUrl { get; set; }
		public string PetSpecies { get; set; } = string.Empty;
		public string PetBreed { get; set; } = string.Empty;
		public int PetAge { get; set; }

		public string PetDescription { get; set; } = string.Empty;

	}
}
