using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TailMates.Web.ViewModels.Pet;

namespace TailMates.Web.ViewModels.Shelter
{
    public class ShelterDetailsViewModel
    {
		public int Id { get; set; }

		[Display(Name = "Shelter Name")]
		public string Name { get; set; } = string.Empty;

		public string Address { get; set; } = string.Empty;

		[Display(Name = "Phone Number")]
		public string? PhoneNumber { get; set; }

		public string Description { get; set; } = null!;

		public string? ImageUrl { get; set; }

		public string? Email { get; set; }

		[Display(Name = "Pets at this Shelter")]
		public IEnumerable<PetViewModel> Pets { get; set; } = new List<PetViewModel>();

	}
}
