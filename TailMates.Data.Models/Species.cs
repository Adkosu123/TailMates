using System.Xml;

namespace TailMates.Data.Models
{
	public class Species
	{
		public int Id { get; set; }

		public string Name { get; set; } = null!;

		public virtual ICollection<Pet> Pets { get; set; }
		     = new HashSet<Pet>();

		public virtual ICollection<Breed> Breeds { get; set; }
		     =  new HashSet<Breed>();
	}
}