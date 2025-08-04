using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TailMates.Data.Models;
using TailMates.Data.Models.Enums;
using TailMates.Data.Repositories.Interfaces;
using TailMates.Services.Core.Interfaces;
using TailMates.Web.ViewModels.Admin;

namespace TailMates.Services.Core.Services
{
	public class AdminService : IAdminService
	{
		private readonly IAdoptionApplicationRepository adoptionApplicationRepository;
		private readonly IPetRepository petRepository;
		public AdminService(IAdoptionApplicationRepository adoptionApplicationRepository,
			IPetRepository petRepository)
		{
			this.adoptionApplicationRepository = adoptionApplicationRepository;
			this.petRepository = petRepository;
		}

		public async Task<PaginatedList<AdminAdoptionApplicationViewModel>> GetAllApplicationsAsync(int pageIndex, int pageSize)
		{
			var applicationsQuery = this.adoptionApplicationRepository
				 .GetAll()
				 .IgnoreQueryFilters()
				 .Include(a => a.Pet)
					.ThenInclude(p => p.Shelter)
				 .Include(a => a.ApplicationUser)
				 .OrderByDescending(a => a.ApplicationDate);

			var paginatedApplications = await PaginatedList<AdoptionApplication>.CreateAsync(applicationsQuery, pageIndex, pageSize);

			var viewModelList = paginatedApplications.Select(a => new AdminAdoptionApplicationViewModel
			{
				Id = a.Id,
				ApplicantId = a.ApplicantId,
				ApplicantName = a.ApplicationUser.UserName,
				PetName = a.Pet.Name,
				PetImageUrl = a.Pet.ImageUrl,
				ShelterName = a.Pet.Shelter.Name,
				ApplicationDate = a.ApplicationDate,
				Status = a.Status,
				ApplicantNotes = a.ApplicantNotes
			}).ToList();

			return new PaginatedList<AdminAdoptionApplicationViewModel>(
				viewModelList,
				paginatedApplications.TotalCount,
				paginatedApplications.PageIndex,
				paginatedApplications.PageSize
			);
		}

		public async Task<AdoptionApplicationDetailsViewModel> GetApplicationDetailsAsync(int applicationId)
		{
			var application = await this.adoptionApplicationRepository
				.GetAll()
				.IgnoreQueryFilters() 
				.Include(a => a.Pet)
					.ThenInclude(p => p.Shelter)
				.Include(a => a.ApplicationUser)
				.FirstOrDefaultAsync(a => a.Id == applicationId);

			if (application == null)
			{
				return null;
			}

			return new AdoptionApplicationDetailsViewModel
			{
				Id = application.Id,
				ApplicationDate = application.ApplicationDate,
				Status = application.Status,
				ApplicantNotes = application.ApplicantNotes,
				AdminNotes = application.AdminNotes,
				ApplicantId = application.ApplicantId,
				ApplicantName = application.ApplicationUser.UserName,
				ApplicantEmail = application.ApplicationUser.Email,
				PetId = application.PetId,
				PetName = application.Pet.Name,
				PetImageUrl = application.Pet.ImageUrl,
				ShelterName = application.Pet.Shelter.Name
			};
		}

		public async Task<bool> UpdateApplicationStatusAndNotesAsync(int applicationId, ApplicationStatus newStatus, string adminNotes)
		{
			var application = await this.adoptionApplicationRepository
				.GetAll()
				.IgnoreQueryFilters() 
				.Include(a => a.Pet)
				.FirstOrDefaultAsync(a => a.Id == applicationId);

			if (application == null)
			{
				return false;
			}

			application.Status = newStatus;
			application.AdminNotes = adminNotes;

			if (newStatus == ApplicationStatus.Approved)
			{
				var pet = await this.petRepository.GetByIdAsync(application.PetId);
				if (pet != null)
				{
					pet.IsDeleted = true;
					this.petRepository.Update(pet);
					var otherApplications = this.adoptionApplicationRepository
						.GetAll()
						.IgnoreQueryFilters() 
						.Where(a => a.PetId == application.PetId && a.Id != applicationId && a.Status == ApplicationStatus.Pending);

					foreach (var app in otherApplications)
					{
						app.Status = ApplicationStatus.Rejected;
					}
				}
			}

			this.adoptionApplicationRepository.Update(application);
			await this.adoptionApplicationRepository.SaveChangesAsync();

			return true;
		}
	}
}
