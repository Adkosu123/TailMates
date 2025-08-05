using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TailMates.Web.ViewModels.Admin
{
	public class UserViewModel
	{
		public string Id { get; set; } = null!;
		public string Username { get; set; } = null!;
		public string Email { get; set; } = null!;
		public IEnumerable<string> Roles { get; set; } = new List<string>();
		public int? ManagedShelterId { get; set; }
		public string? ManagedShelterName { get; set; }
	}
}
