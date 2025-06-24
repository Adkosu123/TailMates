namespace TailMates.Data.Models
{
	public class Breed
	{
		public int Id { get; set; }

		public string Name { get; set; } = null!;

		public int SpeciesId { get; set; }

		public Species Species { get; set; } = null!;

		public virtual ICollection<Pet> Pets { get; set; }
		     = new HashSet<Pet>();
	}
}