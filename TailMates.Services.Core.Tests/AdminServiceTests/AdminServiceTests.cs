// File: TailMates.Services.Core.Tests/AdminServiceTests.cs

using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using TailMates.Services.Core.Services;
using TailMates.Services.Core.Interfaces;
using TailMates.Data.Repositories.Interfaces;
using TailMates.Data.Models;
using TailMates.Data.Models.Enums; // For ApplicationStatus
using TailMates.Web.ViewModels.Admin;
using TailMates.Data; // For PaginatedList
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering; // For SelectList, SelectListItem
using Microsoft.EntityFrameworkCore; // For Include, ThenInclude, IgnoreQueryFilters

using MockQueryable.Moq; // For BuildMock()

namespace TailMates.Services.Core.Tests.AdminServiceTests
{
	public class AdminServiceTests
	{
		private readonly Mock<IAdoptionApplicationRepository> _mockAdoptionApplicationRepository;
		private readonly Mock<IPetRepository> _mockPetRepository;
		private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
		private readonly Mock<RoleManager<IdentityRole>> _mockRoleManager;
		private readonly Mock<IShelterRepository> _mockShelterRepository;
		private readonly AdminService _adminService;

		public AdminServiceTests()
		{
			_mockAdoptionApplicationRepository = new Mock<IAdoptionApplicationRepository>();
			_mockPetRepository = new Mock<IPetRepository>();
			_mockShelterRepository = new Mock<IShelterRepository>();

			// Mock UserManager requires a Mock<IUserStore<ApplicationUser>>
			var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
			_mockUserManager = new Mock<UserManager<ApplicationUser>>(
				userStoreMock.Object, null, null, null, null, null, null, null, null);

			// Mock RoleManager requires a Mock<IRoleStore<IdentityRole>>
			var roleStoreMock = new Mock<IRoleStore<IdentityRole>>();
			_mockRoleManager = new Mock<RoleManager<IdentityRole>>(
				roleStoreMock.Object, null, null, null, null);

			_adminService = new AdminService(
				_mockAdoptionApplicationRepository.Object,
				_mockPetRepository.Object,
				_mockRoleManager.Object,
				_mockShelterRepository.Object,
				_mockUserManager.Object);
		}

		[Fact]
		public async Task GetAllApplicationsAsync_ShouldReturnPaginatedList_OfAllApplications()
		{
			// Arrange
			var applications = new List<AdoptionApplication>
			{
				new AdoptionApplication
				{
					Id = 1, ApplicationDate = DateTime.UtcNow.AddDays(-10), Status = ApplicationStatus.Pending,
					ApplicantNotes = "App1 Notes", AdminNotes = "App1 Admin Notes",
					ApplicantId = "user1", ApplicationUser = new ApplicationUser { Id = "user1", UserName = "user1" },
					Pet = new Pet { Id = 101, Name = "Pet1", ImageUrl = "pet1.jpg", Shelter = new Shelter { Name = "Shelter A" } }
				},
				new AdoptionApplication
				{
					Id = 2, ApplicationDate = DateTime.UtcNow.AddDays(-5), Status = ApplicationStatus.Approved,
					ApplicantNotes = "App2 Notes", AdminNotes = "App2 Admin Notes",
					ApplicantId = "user2", ApplicationUser = new ApplicationUser { Id = "user2", UserName = "user2" },
					Pet = new Pet { Id = 102, Name = "Pet2", ImageUrl = "pet2.jpg", Shelter = new Shelter { Name = "Shelter B" } }
				}
			};

			// Setup repository to return a mock IQueryable for GetAll()
			_mockAdoptionApplicationRepository.Setup(r => r.GetAll())
				.Returns(applications.AsQueryable().BuildMock());

			var pageIndex = 1;
			var pageSize = 10;

			// Act
			var result = await _adminService.GetAllApplicationsAsync(pageIndex, pageSize);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(2, result.Count);
			Assert.Equal(2, result.TotalCount);
			Assert.Equal(pageIndex, result.PageIndex);
			Assert.Equal(pageSize, result.PageSize);

			// Verify mapping and ordering
			Assert.Equal(2, result.First().Id); // Most recent by ApplicationDate
			Assert.Equal("user2", result.First().ApplicantName);
			Assert.Equal("Pet2", result.First().PetName);
			Assert.Equal(ApplicationStatus.Approved, result.First().Status);
		}

		[Fact]
		public async Task GetAllUsersAsync_ShouldReturnPaginatedList_OfAllUsersWithRolesAndShelters()
		{
			// Arrange
			var users = new List<ApplicationUser>
			{
				new ApplicationUser { Id = "user1", UserName = "user1", Email = "user1@test.com", ManagedShelterId = 1, ManagedShelter = new Shelter { Id = 1, Name = "Shelter Alpha" } },
				new ApplicationUser { Id = "user2", UserName = "user2", Email = "user2@test.com", ManagedShelterId = null }
			};

			// Setup UserManager.Users to return a mock IQueryable
			_mockUserManager.Setup(u => u.Users)
				.Returns(users.AsQueryable().BuildMock());

			// Setup GetRolesAsync for each user
			_mockUserManager.Setup(u => u.GetRolesAsync(users[0])).ReturnsAsync(new List<string> { "Admin", "Manager" });
			_mockUserManager.Setup(u => u.GetRolesAsync(users[1])).ReturnsAsync(new List<string> { "User" });

			var pageIndex = 1;
			var pageSize = 10;

			// Act
			var result = await _adminService.GetAllUsersAsync(pageIndex, pageSize);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(2, result.Count);
			Assert.Equal(2, result.TotalCount);
			Assert.Equal(pageIndex, result.PageIndex);
			Assert.Equal(pageSize, result.PageSize);

			// Verify user data and roles
			var user1Vm = result.First(u => u.Id == "user1");
			Assert.Equal("user1", user1Vm.Username);
			Assert.Equal("user1@test.com", user1Vm.Email);
			Assert.Contains("Admin", user1Vm.Roles);
			Assert.Contains("Manager", user1Vm.Roles);
			Assert.Equal(1, user1Vm.ManagedShelterId);
			Assert.Equal("Shelter Alpha", user1Vm.ManagedShelterName);

			var user2Vm = result.First(u => u.Id == "user2");
			Assert.Equal("user2", user2Vm.Username);
			Assert.Equal("user2@test.com", user2Vm.Email);
			Assert.Contains("User", user2Vm.Roles);
			Assert.Null(user2Vm.ManagedShelterId);
			Assert.Null(user2Vm.ManagedShelterName);
		}

		[Fact]
		public async Task GetApplicationDetailsAsync_ShouldReturnDetails_WhenApplicationExists()
		{
			// Arrange
			var applicationId = 1;
			var application = new AdoptionApplication
			{
				Id = applicationId,
				ApplicationDate = DateTime.UtcNow,
				Status = ApplicationStatus.Pending,
				ApplicantNotes = "Applicant details",
				AdminNotes = "Admin details",
				ApplicantId = "user1",
				ApplicationUser = new ApplicationUser { Id = "user1", UserName = "user1", Email = "user1@test.com" },
				Pet = new Pet { Id = 101, Name = "Pet1", ImageUrl = "pet1.jpg", Shelter = new Shelter { Name = "Shelter A" } }
			};

			_mockAdoptionApplicationRepository.Setup(r => r.GetAll())
				.Returns(new List<AdoptionApplication> { application }.AsQueryable().BuildMock());

			// Act
			var result = await _adminService.GetApplicationDetailsAsync(applicationId);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(applicationId, result.Id);
			Assert.Equal(ApplicationStatus.Pending, result.Status);
			Assert.Equal("user1", result.ApplicantName);
			Assert.Equal("user1@test.com", result.ApplicantEmail);
			Assert.Equal("Pet1", result.PetName);
			Assert.Equal("Shelter A", result.ShelterName);
		}

		[Fact]
		public async Task GetApplicationDetailsAsync_ShouldReturnNull_WhenApplicationDoesNotExist()
		{
			// Arrange
			var applicationId = 99;
			_mockAdoptionApplicationRepository.Setup(r => r.GetAll())
				.Returns(new List<AdoptionApplication>().AsQueryable().BuildMock());

			// Act
			var result = await _adminService.GetApplicationDetailsAsync(applicationId);

			// Assert
			Assert.Null(result);
		}

		[Fact]
		public async Task GetUserRolesAndShelterAsync_ShouldReturnViewModel_WhenUserExists()
		{
			// Arrange
			var userId = "testUser";
			var user = new ApplicationUser { Id = userId, UserName = "testUser", Email = "test@test.com", ManagedShelterId = 1 };
			var currentRoles = new List<string> { "User", "Manager" };
			var allRoleNames = new List<string> { "Admin", "Manager", "User" };

			var identityRoles = allRoleNames.Select(name => new IdentityRole(name)).AsQueryable().BuildMock();
			_mockRoleManager.Setup(r => r.Roles).Returns(identityRoles);

			var allShelters = new List<Shelter> { new Shelter { Id = 1, Name = "Shelter A" }, new Shelter { Id = 2, Name = "Shelter B" } };

			_mockUserManager.Setup(u => u.FindByIdAsync(userId)).ReturnsAsync(user);
			_mockUserManager.Setup(u => u.GetRolesAsync(user)).ReturnsAsync(currentRoles);

			// --- CRITICAL FIX: Direct mock setup for IEnumerable<Shelter> ---
			// This ensures the mock returns the concrete list directly.
			_mockShelterRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(allShelters);
			// --- END CRITICAL FIX ---

			// Act
			var result = await _adminService.GetUserRolesAndShelterAsync(userId);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(userId, result.UserId);
			Assert.Equal("testUser", result.Username);
			Assert.Equal("test@test.com", result.Email);
			Assert.Equal(currentRoles.Count, result.CurrentRoles.Count);
			Assert.Contains("User", result.CurrentRoles);
			Assert.Contains("Manager", result.CurrentRoles);

			Assert.Equal(3, result.AvailableRoles.Items.OfType<SelectListItem>().Count());
			Assert.True(result.AvailableRoles.Items.OfType<SelectListItem>().Any(li => li.Text == "Admin"));
			Assert.True(result.AvailableRoles.Items.OfType<SelectListItem>().Any(li => li.Text == "Manager"));
			Assert.True(result.AvailableRoles.Items.OfType<SelectListItem>().Any(li => li.Text == "User"));

			Assert.Equal(1, result.ManagedShelterId);

			// --- VERIFICATION STEP ---
			// Verify that GetAllAsync was indeed called on the mock.
			_mockShelterRepository.Verify(r => r.GetAllAsync(), Times.Once);
			// --- END VERIFICATION STEP ---

			// This is the line that was failing.
			Assert.Equal(2, result.AllShelters.Items.OfType<SelectListItem>().Count());
			Assert.True(result.AllShelters.Items.OfType<SelectListItem>().Any(i => i.Value == "1" && i.Selected));
		}

		[Fact]
		public async Task GetUserRolesAndShelterAsync_ShouldReturnNull_WhenUserDoesNotExist()
		{
			// Arrange
			var userId = "nonExistentUser";
			_mockUserManager.Setup(u => u.FindByIdAsync(userId)).ReturnsAsync((ApplicationUser)null);

			// Act
			var result = await _adminService.GetUserRolesAndShelterAsync(userId);

			// Assert
			Assert.Null(result);
			_mockUserManager.Verify(u => u.GetRolesAsync(It.IsAny<ApplicationUser>()), Times.Never);
			_mockRoleManager.Verify(r => r.Roles, Times.Never);
			_mockShelterRepository.Verify(r => r.GetAllAsync(), Times.Never);
		}

		[Fact]
		public async Task UpdateApplicationStatusAndNotesAsync_ShouldUpdateStatusAndNotes_WhenApplicationExists()
		{
			// Arrange
			var applicationId = 1;
			var application = new AdoptionApplication
			{
				Id = applicationId,
				Status = ApplicationStatus.Pending,
				PetId = 101,
				Pet = new Pet { Id = 101, IsDeleted = false } // Pet must be included for service logic
			};
			var newStatus = ApplicationStatus.Approved;
			var adminNotes = "Approved by admin.";

			_mockAdoptionApplicationRepository.Setup(r => r.GetAll())
				.Returns(new List<AdoptionApplication> { application }.AsQueryable().BuildMock());
			_mockPetRepository.Setup(r => r.GetByIdAsync(application.PetId)).ReturnsAsync(application.Pet);
			_mockPetRepository.Setup(r => r.Update(It.IsAny<Pet>()));
			_mockAdoptionApplicationRepository.Setup(r => r.Update(It.IsAny<AdoptionApplication>()));
			_mockAdoptionApplicationRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

			// Mock other applications for the same pet to be rejected
			var otherPendingApp = new AdoptionApplication { Id = 2, PetId = 101, Status = ApplicationStatus.Pending };
			_mockAdoptionApplicationRepository.Setup(r => r.GetAll())
				.Returns(new List<AdoptionApplication> { application, otherPendingApp }.AsQueryable().BuildMock());


			// Act
			var result = await _adminService.UpdateApplicationStatusAndNotesAsync(applicationId, newStatus, adminNotes);

			// Assert
			Assert.True(result);
			Assert.Equal(newStatus, application.Status);
			Assert.Equal(adminNotes, application.AdminNotes);
			Assert.True(application.Pet.IsDeleted); // Pet should be marked as deleted
			Assert.Equal(ApplicationStatus.Rejected, otherPendingApp.Status); // Other pending app should be rejected

			_mockAdoptionApplicationRepository.Verify(r => r.Update(application), Times.Once);
			_mockPetRepository.Verify(r => r.Update(application.Pet), Times.Once);
			_mockAdoptionApplicationRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
		}

		[Fact]
		public async Task UpdateApplicationStatusAndNotesAsync_ShouldReturnFalse_WhenApplicationDoesNotExist()
		{
			// Arrange
			var applicationId = 99;
			_mockAdoptionApplicationRepository.Setup(r => r.GetAll())
				.Returns(new List<AdoptionApplication>().AsQueryable().BuildMock());

			// Act
			var result = await _adminService.UpdateApplicationStatusAndNotesAsync(applicationId, ApplicationStatus.Approved, "Notes");

			// Assert
			Assert.False(result);
			_mockAdoptionApplicationRepository.Verify(r => r.Update(It.IsAny<AdoptionApplication>()), Times.Never);
			_mockPetRepository.Verify(r => r.Update(It.IsAny<Pet>()), Times.Never);
			_mockAdoptionApplicationRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
		}

		[Fact]
		public async Task UpdateApplicationStatusAndNotesAsync_ShouldNotDeletePetOrRejectOthers_WhenStatusIsNotApproved()
		{
			// Arrange
			var applicationId = 1;
			var application = new AdoptionApplication
			{
				Id = applicationId,
				Status = ApplicationStatus.Pending,
				PetId = 101,
				Pet = new Pet { Id = 101, IsDeleted = false }
			};
			var newStatus = ApplicationStatus.Rejected; // Not Approved
			var adminNotes = "Rejected by admin.";

			_mockAdoptionApplicationRepository.Setup(r => r.GetAll())
				.Returns(new List<AdoptionApplication> { application }.AsQueryable().BuildMock());

			_mockAdoptionApplicationRepository.Setup(r => r.Update(It.IsAny<AdoptionApplication>()));
			_mockAdoptionApplicationRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

			// Act
			var result = await _adminService.UpdateApplicationStatusAndNotesAsync(applicationId, newStatus, adminNotes);

			// Assert
			Assert.True(result);
			Assert.Equal(newStatus, application.Status);
			Assert.Equal(adminNotes, application.AdminNotes);
			Assert.False(application.Pet.IsDeleted); // Pet should NOT be marked as deleted

			_mockPetRepository.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never); // Pet repo methods not called
			_mockPetRepository.Verify(r => r.Update(It.IsAny<Pet>()), Times.Never);
			_mockAdoptionApplicationRepository.Verify(r => r.Update(application), Times.Once); // The main application should be updated
			_mockAdoptionApplicationRepository.Verify(r => r.SaveChangesAsync(), Times.Once);

			_mockAdoptionApplicationRepository.Verify(
				r => r.Update(It.Is<AdoptionApplication>(app => app.Id != application.Id)),
				Times.Never,
				"Update should not be called on other applications when status is not Approved."
			);
			// --- FIX END ---
		}

		[Fact]
		public async Task UpdateUserRolesAndShelterAsync_ShouldUpdateRolesAndShelter_WhenUserExists()
		{
			// Arrange
			var userId = "user1";
			var user = new ApplicationUser { Id = userId, UserName = "user1", ManagedShelterId = null };
			var currentRoles = new List<string> { "User" }; // User has "User" role
			var selectedRoles = new List<string> { "User", "Manager" }; // Wants to keep "User", add "Manager"
			int? managedShelterId = 5;

			_mockUserManager.Setup(u => u.FindByIdAsync(userId)).ReturnsAsync(user);
			_mockUserManager.Setup(u => u.GetRolesAsync(user)).ReturnsAsync(currentRoles);
			_mockUserManager.Setup(u => u.RemoveFromRolesAsync(user, It.IsAny<IEnumerable<string>>())).ReturnsAsync(IdentityResult.Success);
			_mockUserManager.Setup(u => u.AddToRolesAsync(user, It.IsAny<IEnumerable<string>>())).ReturnsAsync(IdentityResult.Success);
			_mockUserManager.Setup(u => u.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

			// Act
			var result = await _adminService.UpdateUserRolesAndShelterAsync(userId, selectedRoles, managedShelterId);

			// Assert
			Assert.True(result);
			Assert.Equal(managedShelterId, user.ManagedShelterId);


			_mockUserManager.Verify(u => u.RemoveFromRolesAsync(user, It.IsAny<IEnumerable<string>>()), Times.Never);
			// --- FIX END ---

			_mockUserManager.Verify(u => u.AddToRolesAsync(user, It.Is<List<string>>(r => r.Contains("Manager") && r.Count == 1)), Times.Once); // Manager role added
			_mockUserManager.Verify(u => u.UpdateAsync(user), Times.Once);
		}

		[Fact]
		public async Task UpdateUserRolesAndShelterAsync_ShouldReturnFalse_WhenUserDoesNotExist()
		{
			// Arrange
			var userId = "nonExistent";
			_mockUserManager.Setup(u => u.FindByIdAsync(userId)).ReturnsAsync((ApplicationUser)null);

			// Act
			var result = await _adminService.UpdateUserRolesAndShelterAsync(userId, new List<string>(), null);

			// Assert
			Assert.False(result);
			_mockUserManager.Verify(u => u.GetRolesAsync(It.IsAny<ApplicationUser>()), Times.Never);
			_mockUserManager.Verify(u => u.UpdateAsync(It.IsAny<ApplicationUser>()), Times.Never);
		}

		[Fact]
		public async Task UpdateUserRolesAndShelterAsync_ShouldReturnFalse_WhenRoleRemovalFails()
		{
			// Arrange
			var userId = "user1";
			var user = new ApplicationUser { Id = userId, UserName = "user1" };
			var currentRoles = new List<string> { "User", "Manager" };
			var selectedRoles = new List<string> { "User" }; // Manager role to be removed

			_mockUserManager.Setup(u => u.FindByIdAsync(userId)).ReturnsAsync(user);
			_mockUserManager.Setup(u => u.GetRolesAsync(user)).ReturnsAsync(currentRoles);
			_mockUserManager.Setup(u => u.RemoveFromRolesAsync(user, It.IsAny<IEnumerable<string>>())).ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Failed" }));

			// Act
			var result = await _adminService.UpdateUserRolesAndShelterAsync(userId, selectedRoles, null);

			// Assert
			Assert.False(result);
			_mockUserManager.Verify(u => u.RemoveFromRolesAsync(user, It.Is<List<string>>(r => r.Contains("Manager"))), Times.Once);
			_mockUserManager.Verify(u => u.AddToRolesAsync(It.IsAny<ApplicationUser>(), It.IsAny<IEnumerable<string>>()), Times.Never);
			_mockUserManager.Verify(u => u.UpdateAsync(It.IsAny<ApplicationUser>()), Times.Never);
		}

		[Fact]
		public async Task UpdateUserRolesAndShelterAsync_ShouldReturnFalse_WhenRoleAdditionFails()
		{
			// Arrange
			var userId = "user1";
			var user = new ApplicationUser { Id = userId, UserName = "user1" };
			var currentRoles = new List<string> { "User" };
			var selectedRoles = new List<string> { "User", "Admin" }; // Admin role to be added

			_mockUserManager.Setup(u => u.FindByIdAsync(userId)).ReturnsAsync(user);
			_mockUserManager.Setup(u => u.GetRolesAsync(user)).ReturnsAsync(currentRoles);
			_mockUserManager.Setup(u => u.RemoveFromRolesAsync(user, It.IsAny<IEnumerable<string>>())).ReturnsAsync(IdentityResult.Success);
			_mockUserManager.Setup(u => u.AddToRolesAsync(user, It.IsAny<IEnumerable<string>>())).ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Failed" }));

			// Act
			var result = await _adminService.UpdateUserRolesAndShelterAsync(userId, selectedRoles, null);

			// Assert
			Assert.False(result);
			_mockUserManager.Verify(u => u.AddToRolesAsync(user, It.Is<List<string>>(r => r.Contains("Admin"))), Times.Once);
			_mockUserManager.Verify(u => u.UpdateAsync(It.IsAny<ApplicationUser>()), Times.Never);
		}

		[Fact]
		public async Task UpdateUserRolesAndShelterAsync_ShouldReturnFalse_WhenUserUpdateFails()
		{
			// Arrange
			var userId = "user1";
			var user = new ApplicationUser { Id = userId, UserName = "user1" };
			var currentRoles = new List<string> { "User" };
			var selectedRoles = new List<string> { "User" };
			int? managedShelterId = 5; // Change managed shelter

			_mockUserManager.Setup(u => u.FindByIdAsync(userId)).ReturnsAsync(user);
			_mockUserManager.Setup(u => u.GetRolesAsync(user)).ReturnsAsync(currentRoles);
			_mockUserManager.Setup(u => u.RemoveFromRolesAsync(user, It.IsAny<IEnumerable<string>>())).ReturnsAsync(IdentityResult.Success);
			_mockUserManager.Setup(u => u.AddToRolesAsync(user, It.IsAny<IEnumerable<string>>())).ReturnsAsync(IdentityResult.Success);
			_mockUserManager.Setup(u => u.UpdateAsync(user)).ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Failed" }));

			// Act
			var result = await _adminService.UpdateUserRolesAndShelterAsync(userId, selectedRoles, managedShelterId);

			// Assert
			Assert.False(result);
			_mockUserManager.Verify(u => u.UpdateAsync(user), Times.Once);
		}

		[Fact]
		public async Task DeleteUserAsync_ShouldReturnTrue_WhenUserIsDeletedSuccessfully()
		{
			// Arrange
			var userId = "userToDelete";
			var user = new ApplicationUser { Id = userId, UserName = "deleteMe" };

			_mockUserManager.Setup(u => u.FindByIdAsync(userId)).ReturnsAsync(user);
			_mockUserManager.Setup(u => u.DeleteAsync(user)).ReturnsAsync(IdentityResult.Success);

			// Act
			var result = await _adminService.DeleteUserAsync(userId);

			// Assert
			Assert.True(result);
			_mockUserManager.Verify(u => u.FindByIdAsync(userId), Times.Once);
			_mockUserManager.Verify(u => u.DeleteAsync(user), Times.Once);
		}

		[Fact]
		public async Task DeleteUserAsync_ShouldReturnFalse_WhenUserDoesNotExist()
		{
			// Arrange
			var userId = "nonExistentUser";
			_mockUserManager.Setup(u => u.FindByIdAsync(userId)).ReturnsAsync((ApplicationUser)null);

			// Act
			var result = await _adminService.DeleteUserAsync(userId);

			// Assert
			Assert.False(result);
			_mockUserManager.Verify(u => u.FindByIdAsync(userId), Times.Once);
			_mockUserManager.Verify(u => u.DeleteAsync(It.IsAny<ApplicationUser>()), Times.Never);
		}

		[Fact]
		public async Task DeleteUserAsync_ShouldReturnFalse_WhenDeleteFails()
		{
			// Arrange
			var userId = "userToDelete";
			var user = new ApplicationUser { Id = userId, UserName = "deleteMe" };

			_mockUserManager.Setup(u => u.FindByIdAsync(userId)).ReturnsAsync(user);
			_mockUserManager.Setup(u => u.DeleteAsync(user)).ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Failed to delete" }));

			// Act
			var result = await _adminService.DeleteUserAsync(userId);

			// Assert
			Assert.False(result);
			_mockUserManager.Verify(u => u.FindByIdAsync(userId), Times.Once);
			_mockUserManager.Verify(u => u.DeleteAsync(user), Times.Once);
		}
	}
}