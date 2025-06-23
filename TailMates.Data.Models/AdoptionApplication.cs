using System.Runtime.CompilerServices;

namespace TailMates.Data.Models
{
	public class AdoptionApplication
	{
		public int Id { get; set; }

		public string Message { get; set; } = null!;

		public string Status { get; set; } = null!;

		public DateTime SubmittedOn { get; set; }

		public string UserId { get; set; } = null!;

		public ApplicationUser User { get; set; } = null!;

		public int PetId { get; set; }

		public Pet Pet { get; set; } = null!;
	}
}