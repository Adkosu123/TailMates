using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TailMates.Web.ViewModels.Pet
{
   public class AdoptionApplicationViewModel
    {
		public int Id { get; set; }

		[Display(Name = "Application Date")]
		[DataType(DataType.Date)]
		public DateTime ApplicationDate { get; set; }

		public string Status { get; set; } = string.Empty; 

		[Display(Name = "Applicant Name")]
		public string ApplicantName { get; set; } = string.Empty;

		public string? ApplicantNotes { get; set; }
		public string? AdminNotes { get; set; }
	}
}
