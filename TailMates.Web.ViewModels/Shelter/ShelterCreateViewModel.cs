using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TailMates.Data.Common.ValidationConstants.Shelter;

namespace TailMates.Web.ViewModels.Shelter
{
	public class ShelterCreateViewModel
	{
		[Required(ErrorMessage = "Shelter name is required.")]
		[StringLength(ShelterNameMaxLength, MinimumLength = ShelterNameMinLength,
	    ErrorMessage = "Name must be between {2} and {1} characters long.")]
		[Display(Name = "Shelter Name")]
		public string Name { get; set; } = null!;

		[Required(ErrorMessage = "Address is required.")]
		[StringLength(ShelterAddressMaxLength, MinimumLength = ShelterAddressMinLength,
		ErrorMessage = "Address must be between {2} and {1} characters long.")]
		[Display(Name = "Address")]
		public string Address { get; set; } = null!;

		[Required(ErrorMessage = "Description is required.")]
		[StringLength(ShelterDescriptionMaxLength, MinimumLength = ShelterDescriptionMinLength,
		ErrorMessage = "Description must be between {2} and {1} characters long.")]
		[Display(Name = "Description")]
		public string Description { get; set; } = null!;

		[Display(Name = "Phone Number")]
		[Phone(ErrorMessage = "Please enter a valid phone number.")]
		public string? PhoneNumber { get; set; }

		[Display(Name = "Email")]
		[EmailAddress(ErrorMessage = "Please enter a valid email address.")]
		public string? Email { get; set; }

		[Required(ErrorMessage = "Image URL is required.")]
		[Url(ErrorMessage = "Please enter a valid URL for the image.")]
		[Display(Name = "Image URL")]
		public string ImageUrl { get; set; } = null!;
	}
}
