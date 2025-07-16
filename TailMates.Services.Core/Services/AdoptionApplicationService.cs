using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TailMates.Data;
using TailMates.Data.Models;
using TailMates.Data.Models.Enums;
using TailMates.Data.Repositories.Implementations;
using TailMates.Data.Repositories.Interfaces;
using TailMates.Services.Core.Interfaces;
using TailMates.Web.ViewModels.AdoptionApplication;

namespace TailMates.Services.Core.Services
{
	public class AdoptionApplicationService : IAdoptionApplicationService
	{
		private readonly IAdoptionApplicationRepository adoptionApplicationRepository;
		private readonly IPetRepository petRepository;

		public AdoptionApplicationService(IAdoptionApplicationRepository adoptionApplicationRepository,
			IPetRepository petRepository)
		{
			this.adoptionApplicationRepository = adoptionApplicationRepository;
			this.petRepository = petRepository;
		}

		public async Task<bool> CreateApplicationAsync(AdoptionApplicationCreateViewModel model, string applicantId)
		{

			var pet = await petRepository.GetAvailablePetByIdAsync(model.PetId);

			if (pet == null)
			{
				return false;
			}

			var existingPendingApplication = await adoptionApplicationRepository
												.HasPendingApplicationForPetAndApplicantAsync(model.PetId, applicantId);

			if (existingPendingApplication)
			{
				return false;
			}

			var application = new AdoptionApplication
			{
				PetId = model.PetId,
				ApplicantId = applicantId,
				ApplicationDate = DateTime.UtcNow,
				Status = ApplicationStatus.Pending,
				ApplicantNotes = model.ApplicantNotes
			};

			await adoptionApplicationRepository.AddAsync(application);
			await adoptionApplicationRepository.SaveChangesAsync();  

			return true; 
		}
	}
}
