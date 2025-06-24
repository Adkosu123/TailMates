using TailMates.Data.Models.Enums;

namespace TailMates.Data.Models
{
	public class AdoptionApplication
	{
		public int Id { get; set; }

		public DateTime ApplicationDate { get; set; } = DateTime.UtcNow;

		public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;

		public string? ApplicantNotes { get; set; }

		public string? AdminNotes { get; set; }

		public string ApplicantId { get; set; } = null!;

		public ApplicationUser ApplicationUser { get; set; } = null!;

		public int PetId { get; set; }

		public Pet Pet { get; set; } = null!;
	}
}