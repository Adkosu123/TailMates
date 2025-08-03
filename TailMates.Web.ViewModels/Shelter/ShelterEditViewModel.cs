using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TailMates.Data.Common;
using static TailMates.Data.Common.ValidationConstants;

namespace TailMates.Web.ViewModels.Shelter
{
	public class ShelterEditViewModel
	{
		public int Id { get; set; }

		[Required]
		[StringLength(ValidationConstants.Shelter.ShelterNameMaxLength, MinimumLength = ValidationConstants.Shelter.ShelterNameMinLength)]
		public string Name { get; set; } = null!;

		[Required]
		[StringLength(ValidationConstants.Shelter.ShelterAddressMaxLength, MinimumLength = ValidationConstants.Shelter.ShelterAddressMinLength)]
		public string Address { get; set; } = null!;

		[Required]
		[StringLength(ValidationConstants.Shelter.ShelterDescriptionMaxLength, MinimumLength = ValidationConstants.Shelter.ShelterDescriptionMinLength)]
		public string Description { get; set; } = null!;

		[Required]
		[Phone]
		[StringLength(ValidationConstants.Shelter.ShelterPhoneNumberMaxLength)]
		public string PhoneNumber { get; set; } = null!;

		[Required]
		[EmailAddress]
		[StringLength(ValidationConstants.Shelter.ShelterEmailMaxLength)]
		public string Email { get; set; } = null!;

		[Required]
		[Url]
		public string ImageUrl { get; set; } = null!;
	}
}
