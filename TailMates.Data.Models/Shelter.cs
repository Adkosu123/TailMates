namespace TailMates.Data.Models
{
	public class Shelter
	{
		public int Id { get; set; }

		public string Name { get; set; } = null!;

		public string Address { get; set; } = null!;

		public string Description { get; set; }

		public string? PhoneNumber { get; set; }

		public string? Email { get; set; }

		public string? ImageUrl { get; set; }

		public bool IsDeleted { get; set; }

		public virtual ICollection<Pet> Pets { get; set; }
		       = new HashSet<Pet>();
	}
}