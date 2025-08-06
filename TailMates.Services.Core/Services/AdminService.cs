using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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
		private readonly UserManager<ApplicationUser> userManager;
		private readonly RoleManager<IdentityRole> roleManager;
		private readonly IShelterRepository shelterRepository;
		public AdminService(IAdoptionApplicationRepository adoptionApplicationRepository,
			IPetRepository petRepository,
			RoleManager<IdentityRole> roleManager,
			IShelterRepository shelterRepository,
			UserManager<ApplicationUser> userManager)
		{
			this.adoptionApplicationRepository = adoptionApplicationRepository;
			this.petRepository = petRepository;
			this.roleManager = roleManager;
			this.shelterRepository = shelterRepository;
			this.userManager = userManager;
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

		public async Task<PaginatedList<UserViewModel>> GetAllUsersAsync(int pageIndex, int pageSize)
		{
			var usersQuery = userManager.Users
				.Include(u => u.ManagedShelter) 
				.OrderBy(u => u.UserName)
				.AsQueryable();

			var paginatedUsers = await PaginatedList<ApplicationUser>.CreateAsync(usersQuery, pageIndex, pageSize);

			var userViewModels = new List<UserViewModel>();
			foreach (var user in paginatedUsers)
			{
				var roles = await userManager.GetRolesAsync(user);
				userViewModels.Add(new UserViewModel
				{
					Id = user.Id,
					Username = user.UserName,
					Email = user.Email,
					Roles = roles,
					ManagedShelterId = user.ManagedShelterId,
					ManagedShelterName = user.ManagedShelter?.Name
				});
			}

			return new PaginatedList<UserViewModel>(
				userViewModels,
				paginatedUsers.TotalCount,
				paginatedUsers.PageIndex,
				paginatedUsers.PageSize
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

		public async Task<ManageUserRolesViewModel?> GetUserRolesAndShelterAsync(string userId)
		{
			var user = await userManager.FindByIdAsync(userId);
			if (user == null)
			{
				return null;
			}

			var userRoles = await userManager.GetRolesAsync(user);
			var allRoles = roleManager.Roles.Select(r => r.Name).ToList();
			var allShelters = await shelterRepository.GetAllAsync();

			var availableRolesSelectListItems = allRoles.Select(roleName => new SelectListItem
			{
				Value = roleName,
				Text = roleName
			}).ToList();

			var allSheltersSelectListItems = allShelters.Select(s => new SelectListItem
			{
				Value = s.Id.ToString(),
				Text = s.Name,
				Selected = (user.ManagedShelterId.HasValue && s.Id == user.ManagedShelterId.Value)
			}).ToList();

			var allSheltersSelectList = new SelectList(allSheltersSelectListItems, "Value", "Text");

			var viewModel = new ManageUserRolesViewModel
			{
				UserId = user.Id,
				Username = user.UserName,
				Email = user.Email,
				CurrentRoles = userRoles.ToList(),
				AvailableRoles = new SelectList(availableRolesSelectListItems, "Value", "Text"),
				SelectedRoles = userRoles.ToList(),
				ManagedShelterId = user.ManagedShelterId,
				AllShelters = allSheltersSelectList
			};

			return viewModel;
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

		public async Task<bool> UpdateUserRolesAndShelterAsync(string userId, List<string> selectedRoles, int? managedShelterId)
		{
			var user = await userManager.FindByIdAsync(userId);
			if (user == null)
			{
				return false;
			}

			var currentRoles = await userManager.GetRolesAsync(user);

			var rolesToRemove = currentRoles.Except(selectedRoles).ToList();
			if (rolesToRemove.Any())
			{
				var removeResult = await userManager.RemoveFromRolesAsync(user, rolesToRemove);
				if (!removeResult.Succeeded)
				{
					return false;
				}
			}

			var rolesToAdd = selectedRoles.Except(currentRoles).ToList();
			if (rolesToAdd.Any())
			{
				var addResult = await userManager.AddToRolesAsync(user, rolesToAdd);
				if (!addResult.Succeeded)
				{
					return false;
				}
			}

			user.ManagedShelterId = managedShelterId;
			var updateResult = await userManager.UpdateAsync(user);
			if (!updateResult.Succeeded)
			{
				return false;
			}

			return true;
		}

		public async Task<bool> DeleteUserAsync(string userId)
		{
			var user = await userManager.FindByIdAsync(userId);
			if (user == null)
			{
				return false;
			}

			var result = await userManager.DeleteAsync(user);
			return result.Succeeded;
		}
	}
}
