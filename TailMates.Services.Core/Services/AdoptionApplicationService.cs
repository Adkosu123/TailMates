using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TailMates.Data;
using TailMates.Data.Models;
using TailMates.Data.Models.Enums;
using TailMates.Services.Core.Interfaces;
using TailMates.Web.ViewModels.AdoptionApplication;

namespace TailMates.Services.Core.Services
{
	public class AdoptionApplicationService : IAdoptionApplicationService
	{
		private readonly TailMatesDbContext dbContext;

		public AdoptionApplicationService(TailMatesDbContext dbContext)
		{
			this.dbContext = dbContext;
		}

		public async Task<bool> CreateApplicationAsync(AdoptionApplicationCreateViewModel model, string applicantId)
		{
			var pet = await dbContext.Pets
									 .Where(p => !p.IsDeleted)
									 .FirstOrDefaultAsync(p => p.Id == model.PetId && !p.IsAdopted);

			if (pet == null)
			{
				
				return false;
			}

			var existingPendingApplication = await dbContext.AdoptionApplications
													  .AnyAsync(a => a.PetId == model.PetId && a.ApplicantId == applicantId && a.Status == ApplicationStatus.Pending);

			if (existingPendingApplication)
			{
				return false;
			}

			var application = new AdoptionApplication
			{
				PetId = model.PetId,
				ApplicantId = applicantId, // The ID of the authenticated user
				ApplicationDate = DateTime.UtcNow, // Record the current UTC time of application
				Status = ApplicationStatus.Pending, // Set initial status to Pending
				ApplicantNotes = model.ApplicantNotes // Store notes provided by the applicant
			};

			// 4. Add to DbContext and Save Changes:
			await dbContext.AdoptionApplications.AddAsync(application); // Add the new application to the DbSet
			await dbContext.SaveChangesAsync(); // Persist changes to the database

			return true; // Application created successfully
		}
	}
}
