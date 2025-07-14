using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TailMates.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
		public string? FirstName { get; set; }

		public string? LastName { get; set; }

		public int? ManagedShelterId { get; set; }

		public Shelter? ManagedShelter { get; set; }

		public virtual ICollection<AdoptionApplication> UserAdoptionApplications { get; set; }
			  = new HashSet<AdoptionApplication>();
	}
}
