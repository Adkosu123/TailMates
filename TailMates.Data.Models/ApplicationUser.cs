using Microsoft.AspNetCore.Identity;

namespace TailMates.Data.Models
{
	public class ApplicationUser : IdentityUser
	{
		public string FullName { get; set; } = null!;

		public virtual ICollection<AdoptionApplication> Applications { get; set; }
		      = new HashSet<AdoptionApplication>();

		public virtual ICollection<Pet> Pets { get; set; }
		      = new HashSet<Pet>();
	}
}