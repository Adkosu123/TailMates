using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TailMates.Data.Models.Enums;

namespace TailMates.Web.ViewModels.MyAdoptionApplications
{
	public class AdoptionApplicationViewModel
	{
		public int Id { get; set; }

		public int PetId { get; set; }
		public string PetName { get; set; } = null!;
		public string PetImageUrl { get; set; } = null!;
		public string ShelterName { get; set; } = null!;
		public DateTime ApplicationDate { get; set; }
		public ApplicationStatus Status { get; set; }
		public string? ApplicantNotes { get; set; }
	}
}
