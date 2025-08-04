using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TailMates.Data.Models;
using TailMates.Data.Repositories.Interfaces;
using TailMates.Services.Core.Interfaces;
using TailMates.Web.ViewModels.MyAdoptionApplications;

namespace TailMates.Services.Core.Services
{
	public class MyAdoptionApplicationsService : IMyAdoptionApplicationsService
	{
		private readonly IMyAdoptionApplicationsRepository myAdoptionApplicationsRepository;

		public MyAdoptionApplicationsService(IMyAdoptionApplicationsRepository myAdoptionApplicationsRepository)
		{
			this.myAdoptionApplicationsRepository = myAdoptionApplicationsRepository;
		}

		public async Task<PaginatedList<AdoptionApplicationViewModel>> GetUserApplicationsAsync(string userId, int pageIndex, int pageSize)
		{
			var applicationsQuery = this.myAdoptionApplicationsRepository
				.GetAllByApplicantId(userId)
				.IgnoreQueryFilters() // Add this line to bypass the global IsDeleted filter
				.Include(a => a.Pet)
					.ThenInclude(p => p.Shelter)
				.OrderByDescending(a => a.ApplicationDate);

			var paginatedApplications = await PaginatedList<AdoptionApplication>.CreateAsync(applicationsQuery, pageIndex, pageSize);

			var viewModelList = paginatedApplications.Select(a => new AdoptionApplicationViewModel
			{
				Id = a.Id,
				PetId = a.PetId,
				PetName = a.Pet.Name,
				PetImageUrl = a.Pet.ImageUrl,
				ShelterName = a.Pet.Shelter.Name,
				ApplicationDate = a.ApplicationDate,
				Status = a.Status,
				ApplicantNotes = a.ApplicantNotes,
				AdminNotes = a.AdminNotes
			}).ToList();

			return new PaginatedList<AdoptionApplicationViewModel>(
				viewModelList,
				paginatedApplications.TotalCount,
				paginatedApplications.PageIndex,
				paginatedApplications.PageSize
			);
		}
	}
}
