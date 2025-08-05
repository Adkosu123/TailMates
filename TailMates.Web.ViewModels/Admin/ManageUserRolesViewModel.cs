using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TailMates.Web.ViewModels.Admin
{
	public class ManageUserRolesViewModel
	{
		public string UserId { get; set; } = null!;
    	public string Username { get; set; } = null!;
		public string Email { get; set; } = null!;

		[Display(Name = "Current Roles")]
		public IList<string> CurrentRoles { get; set; } = new List<string>();

		[Display(Name = "Available Roles")]
		public SelectList? AvailableRoles { get; set; }

		[Display(Name = "Select Roles")]
		public List<string> SelectedRoles { get; set; } = new List<string>();

		[Display(Name = "Managed Shelter")]
		public int? ManagedShelterId { get; set; }

		[Display(Name = "All Shelters")]
		public SelectList? AllShelters { get; set; }
	}
}
