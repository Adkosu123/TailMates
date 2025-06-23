using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TailMates.Data.Models
{
    public class PetTag
    {
		public int Id { get; set; }

		public string Name { get; set; } = null!;

		public virtual ICollection<PetPetTag> PetTags { get; set; }
		     = new HashSet<PetPetTag>();
	}
}
