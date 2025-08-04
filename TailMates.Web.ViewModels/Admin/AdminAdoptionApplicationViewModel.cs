using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TailMates.Data.Models.Enums;

namespace TailMates.Web.ViewModels.Admin
{
	public class AdminAdoptionApplicationViewModel
	{
		public int Id { get; set; }
		public string ApplicantId { get; set; } = null!;
		public string ApplicantName { get; set; } = null!;
		public string PetName { get; set; } = null!;
		public string PetImageUrl { get; set; } = null!;
		public string ShelterName { get; set; } = null!;
		public DateTime ApplicationDate { get; set; }
		public ApplicationStatus Status { get; set; }
		public string? ApplicantNotes { get; set; }
	}
}
