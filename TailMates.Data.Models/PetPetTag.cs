namespace TailMates.Data.Models
{
	public class PetPetTag
	{
		public int PetId { get; set; }

		public Pet Pet { get; set; } = null!;

		public int PetTagId { get; set; }

		public PetTag PetTag { get; set; } = null!;
	}
}