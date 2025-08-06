using MockQueryable.Moq;
using Moq;
using TailMates.Data.Models;
using TailMates.Data.Models.Enums;
using TailMates.Data.Repositories.Interfaces;
using TailMates.Services.Core.Services;
using Xunit;

namespace TailMates.Services.Core.Tests.MyAdoptionApplicationsServiceTests
{
	public class MyAdoptionApplicationsServiceTests
	{
		private readonly Mock<IMyAdoptionApplicationsRepository> _mockMyAdoptionApplicationsRepository;
		private readonly MyAdoptionApplicationsService _myAdoptionApplicationsService;

		public MyAdoptionApplicationsServiceTests()
		{
			_mockMyAdoptionApplicationsRepository = new Mock<IMyAdoptionApplicationsRepository>();
			_myAdoptionApplicationsService = new MyAdoptionApplicationsService(
				_mockMyAdoptionApplicationsRepository.Object);
		}

		[Fact]
		public async Task GetUserApplicationsAsync_ShouldReturnPaginatedApplications_ForGivenUser()
		{
			// Arrange
			var userId = "testUser123";
			// This list should ONLY contain applications for 'userId'
			var applicationsForUser = new List<AdoptionApplication>
			{
				new AdoptionApplication
				{
					Id = 1,
					ApplicantId = userId,
					ApplicationDate = DateTime.UtcNow.AddDays(-5),
					Status = ApplicationStatus.Pending,
					ApplicantNotes = "Notes 1",
					AdminNotes = "Admin 1",
					Pet = new Pet { Id = 101, Name = "Buddy", ImageUrl = "buddy.jpg", Shelter = new Shelter { Name = "Shelter A" } }
				},
				new AdoptionApplication
				{
					Id = 2,
					ApplicantId = userId,
					ApplicationDate = DateTime.UtcNow.AddDays(-10),
					Status = ApplicationStatus.Approved,
					ApplicantNotes = "Notes 2",
					AdminNotes = "Admin 2",
					Pet = new Pet { Id = 102, Name = "Lucy", ImageUrl = "lucy.jpg", Shelter = new Shelter { Name = "Shelter B" } }
				}
                // Removed the application for "anotherUser" from this list
            };

			// Mock GetAllByApplicantId to return an IQueryable built ONLY from the applications relevant to the userId
			_mockMyAdoptionApplicationsRepository.Setup(r => r.GetAllByApplicantId(userId))
				.Returns(applicationsForUser.AsQueryable().BuildMock());

			var pageIndex = 1;
			var pageSize = 2;

			// Act
			var result = await _myAdoptionApplicationsService.GetUserApplicationsAsync(userId, pageIndex, pageSize);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(2, result.Count); // Now expecting 2, as only 2 applications for this user are mocked
			Assert.Equal(2, result.TotalCount); // Total count for this user
			Assert.Equal(pageIndex, result.PageIndex);
			Assert.Equal(pageSize, result.PageSize);

			// Verify the mapping to ViewModel and order (most recent first)
			var firstApp = result.First();
			Assert.Equal(1, firstApp.Id);
			Assert.Equal("Buddy", firstApp.PetName);
			Assert.Equal("buddy.jpg", firstApp.PetImageUrl);
			Assert.Equal("Shelter A", firstApp.ShelterName);
			Assert.Equal(ApplicationStatus.Pending, firstApp.Status);

			var secondApp = result.Last();
			Assert.Equal(2, secondApp.Id);
			Assert.Equal("Lucy", secondApp.PetName);
			Assert.Equal("lucy.jpg", secondApp.PetImageUrl);
			Assert.Equal("Shelter B", secondApp.ShelterName);
			Assert.Equal(ApplicationStatus.Approved, secondApp.Status);

			// Verify OrderByDescending (most recent first)
			Assert.True(result.First().ApplicationDate > result.Last().ApplicationDate);
		}


		[Fact]
		public async Task GetUserApplicationsAsync_ShouldReturnEmptyList_WhenNoApplicationsForUser()
		{
			// Arrange
			var userId = "nonExistentUser";
			var applications = new List<AdoptionApplication>().AsQueryable();

			_mockMyAdoptionApplicationsRepository.Setup(r => r.GetAllByApplicantId(userId))
				.Returns(applications.BuildMock());

			// Act
			var result = await _myAdoptionApplicationsService.GetUserApplicationsAsync(userId, 1, 10);

			// Assert
			Assert.NotNull(result);
			Assert.Empty(result);
			Assert.Equal(0, result.TotalCount);
		}

		[Fact]
		public async Task GetUserApplicationsAsync_ShouldHandlePaginationCorrectly()
		{
			// Arrange
			var userId = "paginatedUser";
			var applications = new List<AdoptionApplication>();
			for (int i = 1; i <= 5; i++)
			{
				applications.Add(new AdoptionApplication
				{
					Id = i,
					ApplicantId = userId,
					ApplicationDate = DateTime.UtcNow.AddDays(-i), // Older applications first
					Status = ApplicationStatus.Pending,
					Pet = new Pet { Id = i, Name = $"Pet {i}", Shelter = new Shelter { Name = "Shelter X" } }
				});
			}

			// Order applications by date descending as the service does
			var orderedApplications = applications.OrderByDescending(a => a.ApplicationDate).AsQueryable();

			_mockMyAdoptionApplicationsRepository.Setup(r => r.GetAllByApplicantId(userId))
				.Returns(orderedApplications.BuildMock());

			var pageIndex = 2; // Requesting the second page
			var pageSize = 2;

			// Act
			var result = await _myAdoptionApplicationsService.GetUserApplicationsAsync(userId, pageIndex, pageSize);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(pageSize, result.Count); 
			Assert.Equal(5, result.TotalCount);
			Assert.Equal(pageIndex, result.PageIndex);
			Assert.Equal(pageSize, result.PageSize);

			Assert.Equal(3, result.First().Id); 
			Assert.Equal(4, result.Last().Id);  
		}
	}
}