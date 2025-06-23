using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TailMates.Data.Models
{
   public class Shelter
    {
		public int Id { get; set; }

		public string Name { get; set; } = null!;

		public string Location { get; set; } = null!;

		public virtual ICollection<Pet> Pets { get; set; }
		       = new HashSet<Pet>();
	}
}
