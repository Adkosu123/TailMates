using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TailMates.Data.Models.Enums;

namespace TailMates.Data.Models
{
    public class Pet
    {
		public int Id { get; set; }

		public string Name { get; set; } = null!;

		public int Age { get; set; }

		public string Description { get; set; } = null!;

		public string? ImageUrl { get; set; }

		public PetGender Gender { get; set; }

		public bool IsAdopted { get; set; }

		public DateTime DateListed { get; set; } = DateTime.UtcNow;

		public int ShelterId { get; set; }

		public Shelter Shelter { get; set; } = null!;

		public int SpeciesId { get; set; }

		public Species Species { get; set; } = null!;

		public int BreedId { get; set; }

		public Breed Breed { get; set; } = null!;

		public bool IsDeleted { get; set; }

		public virtual ICollection<AdoptionApplication> AdoptionApplications { get; set; } 
			  = new HashSet<AdoptionApplication>();
	}
}
