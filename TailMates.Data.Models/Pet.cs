using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TailMates.Data.Models
{
   public class Pet
    {
		public int Id { get; set; }
		public string Name { get; set; } = null!;

		public string Species { get; set; } = null!;

		public string Breed { get; set; } = null!;

		public int Age { get; set; }

		public string Size { get; set; } = null!;

		public string Description { get; set; } = null!;

		public string? ImageUrl { get; set; }

		public bool IsAdopted { get; set; }

		public string ShelterId { get; set; } = null!;

		public ApplicationUser Shelter { get; set; } = null!;

		public bool IsDeleted { get; set; }

		public virtual ICollection<AdoptionApplication> Applications { get; set; }
		        = new HashSet<AdoptionApplication>();
	}
}
