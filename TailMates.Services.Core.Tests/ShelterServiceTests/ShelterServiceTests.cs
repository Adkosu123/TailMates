using MockQueryable.Moq;
using Moq;
using TailMates.Data.Models;
using TailMates.Data.Repositories.Interfaces;
using TailMates.Services.Core.Services;
using TailMates.Web.ViewModels.Shelter;
using Xunit;

namespace TailMates.Services.Core.Tests.ShelterServiceTests
{
	public class ShelterServiceTests
	{
		private readonly Mock<IShelterRepository> _mockShelterRepository;
		private readonly Mock<IPetRepository> _mockPetRepository;
		private readonly ShelterService _shelterService;

		public ShelterServiceTests()
		{
			_mockShelterRepository = new Mock<IShelterRepository>();
			_mockPetRepository = new Mock<IPetRepository>();
			_shelterService = new ShelterService(
				_mockShelterRepository.Object,
				_mockPetRepository.Object);
		}

		[Fact]
		public async Task AddShelterAsync_ShouldReturnTrue_WhenShelterIsAddedSuccessfully()
		{
			// Arrange
			var createModel = new ShelterCreateViewModel
			{
				Name = "New Shelter",
				Address = "123 Test St",
				Description = "A test shelter.",
				PhoneNumber = "1234567890",
				Email = "test@example.com",
				ImageUrl = "test.jpg"
			};

			_mockShelterRepository.Setup(r => r.AddAsync(It.IsAny<Shelter>())).Returns(Task.CompletedTask);
			_mockShelterRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1); // Simulate 1 change saved

			// Act
			var result = await _shelterService.AddShelterAsync(createModel);

			// Assert
			Assert.True(result);
			_mockShelterRepository.Verify(r => r.AddAsync(It.IsAny<Shelter>()), Times.Once);
			_mockShelterRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
		}

		[Fact]
		public async Task AddShelterAsync_ShouldReturnFalse_WhenSaveChangesAsyncFails()
		{
			// Arrange
			var createModel = new ShelterCreateViewModel
			{
				Name = "New Shelter",
				Address = "123 Test St",
				Email = "test@example.com"
			};

			_mockShelterRepository.Setup(r => r.AddAsync(It.IsAny<Shelter>())).Returns(Task.CompletedTask);
			_mockShelterRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(0); // Simulate 0 changes saved

			// Act
			var result = await _shelterService.AddShelterAsync(createModel);

			// Assert
			Assert.False(result); // Should return false if save fails
			_mockShelterRepository.Verify(r => r.AddAsync(It.IsAny<Shelter>()), Times.Once);
			_mockShelterRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
		}

		[Fact]
		public async Task GetAllSheltersAsync_ShouldReturnPaginatedList_OfAllShelters()
		{
			// Arrange
			var shelters = new List<Shelter>
			{
				new Shelter { Id = 1, Name = "Shelter A", Address = "Add1", Email = "a@a.com", PhoneNumber = "111", ImageUrl = "url1" },
				new Shelter { Id = 2, Name = "Shelter B", Address = "Add2", Email = "b@b.com", PhoneNumber = "222", ImageUrl = "url2" },
				new Shelter { Id = 3, Name = "Shelter C", Address = "Add3", Email = "c@c.com", PhoneNumber = "333", ImageUrl = "url3" }
			};

			// Mock GetAllSheltersWithPets to return an IQueryable that supports async operations
			_mockShelterRepository.Setup(r => r.GetAllSheltersWithPets())
								  .Returns(shelters.AsQueryable().BuildMock());

			var pageIndex = 1;
			var pageSize = 2;

			// Act
			var result = await _shelterService.GetAllSheltersAsync(pageIndex, pageSize);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(pageSize, result.Count); 
			Assert.Equal(3, result.TotalCount);
			Assert.Equal(pageIndex, result.PageIndex);
			Assert.Equal(pageSize, result.PageSize);
			Assert.Equal("Shelter A", result.First().Name);
			Assert.Equal("Shelter B", result.Last().Name);
		}

		[Fact]
		public async Task GetShelterDetailsWithPaginatedPetsAsync_ShouldReturnDetailsWithPets_WhenShelterExists()
		{
			// Arrange
			var shelterId = 1;
			var shelter = new Shelter
			{
				Id = shelterId,
				Name = "Main Shelter",
				Description = "Desc",
				Address = "Addr",
				Email = "email@shelter.com",
				PhoneNumber = "123",
				ImageUrl = "img.jpg"
			};

			var pets = new List<Pet>
			{
				new Pet { Id = 101, Name = "Doggo", ShelterId = shelterId, Species = new Species { Name = "Dog" }, Breed = new Breed { Name = "Lab" } },
				new Pet { Id = 102, Name = "Kitty", ShelterId = shelterId, Species = new Species { Name = "Cat" }, Breed = new Breed { Name = "Siamese" } }
			};

			_mockShelterRepository.Setup(r => r.GetShelterByIdWithPetsAsync(shelterId)).ReturnsAsync(shelter);
			_mockPetRepository.Setup(r => r.GetPetsByShelterId(shelterId))
							  .Returns(pets.AsQueryable().BuildMock()); // Mock IQueryable for pets

			var pageIndex = 1;
			var pageSize = 10;

			// Act
			var result = await _shelterService.GetShelterDetailsWithPaginatedPetsAsync(shelterId, pageIndex, pageSize);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(shelterId, result.Id);
			Assert.Equal("Main Shelter", result.Name);
			Assert.NotNull(result.Pets);
			Assert.Equal(2, result.Pets.Count);
			Assert.Equal("Doggo", result.Pets.First().Name);
			Assert.Equal("Kitty", result.Pets.Last().Name);
			Assert.Equal(2, result.Pets.TotalCount);
		}

		[Fact]
		public async Task GetShelterDetailsWithPaginatedPetsAsync_ShouldReturnNull_WhenShelterDoesNotExist()
		{
			// Arrange
			var shelterId = 99;
			_mockShelterRepository.Setup(r => r.GetShelterByIdWithPetsAsync(shelterId)).ReturnsAsync((Shelter)null);

			// Act
			var result = await _shelterService.GetShelterDetailsWithPaginatedPetsAsync(shelterId, 1, 10);

			// Assert
			Assert.Null(result);
			_mockPetRepository.Verify(r => r.GetPetsByShelterId(It.IsAny<int>()), Times.Never); // Should not try to get pets
		}

		[Fact]
		public async Task GetShelterForEditAsync_ShouldReturnViewModel_WhenShelterExists()
		{
			// Arrange
			var shelterId = 1;
			var shelter = new Shelter
			{
				Id = shelterId,
				Name = "Edit Shelter",
				Address = "Edit Addr",
				Description = "Edit Desc",
				PhoneNumber = "123",
				Email = "edit@example.com",
				ImageUrl = "edit.jpg"
			};
			_mockShelterRepository.Setup(r => r.GetByIdAsync(shelterId)).ReturnsAsync(shelter);

			// Act
			var result = await _shelterService.GetShelterForEditAsync(shelterId);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(shelterId, result.Id);
			Assert.Equal("Edit Shelter", result.Name);
			Assert.Equal("Edit Addr", result.Address);
		}

		[Fact]
		public async Task GetShelterForEditAsync_ShouldReturnNull_WhenShelterDoesNotExist()
		{
			// Arrange
			var shelterId = 99;
			_mockShelterRepository.Setup(r => r.GetByIdAsync(shelterId)).ReturnsAsync((Shelter)null);

			// Act
			var result = await _shelterService.GetShelterForEditAsync(shelterId);

			// Assert
			Assert.Null(result);
		}

		[Fact]
		public async Task UpdateShelterAsync_ShouldReturnTrue_WhenShelterExistsAndIsUpdated()
		{
			// Arrange
			var existingShelter = new Shelter { Id = 1, Name = "Old Name", Address = "Old Address" };
			var updateModel = new ShelterEditViewModel
			{
				Id = 1,
				Name = "Updated Name",
				Address = "New Address",
				Description = "New Desc",
				PhoneNumber = "987",
				Email = "updated@example.com",
				ImageUrl = "updated.jpg"
			};

			_mockShelterRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingShelter);
			_mockShelterRepository.Setup(r => r.Update(It.IsAny<Shelter>())); // No return for void method
			_mockShelterRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

			// Act
			var result = await _shelterService.UpdateShelterAsync(updateModel);

			// Assert
			Assert.True(result);
			Assert.Equal("Updated Name", existingShelter.Name); // Verify entity updated
			Assert.Equal("New Address", existingShelter.Address);
			_mockShelterRepository.Verify(r => r.Update(existingShelter), Times.Once); // Verify Update was called with the modified entity
			_mockShelterRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
		}

		[Fact]
		public async Task UpdateShelterAsync_ShouldReturnFalse_WhenShelterDoesNotExist()
		{
			// Arrange
			var updateModel = new ShelterEditViewModel { Id = 99, Name = "NonExistent" };
			_mockShelterRepository.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Shelter)null);

			// Act
			var result = await _shelterService.UpdateShelterAsync(updateModel);

			// Assert
			Assert.False(result);
			_mockShelterRepository.Verify(r => r.Update(It.IsAny<Shelter>()), Times.Never); // Should not update
			_mockShelterRepository.Verify(r => r.SaveChangesAsync(), Times.Never); // Should not save
		}

		[Fact]
		public async Task UpdateShelterAsync_ShouldReturnFalse_WhenSaveChangesAsyncFails()
		{
			// Arrange
			var existingShelter = new Shelter { Id = 1, Name = "Old Name" };
			var updateModel = new ShelterEditViewModel { Id = 1, Name = "Updated Name" };

			_mockShelterRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingShelter);
			_mockShelterRepository.Setup(r => r.Update(It.IsAny<Shelter>()));
			_mockShelterRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(0); // Simulate failure

			// Act
			var result = await _shelterService.UpdateShelterAsync(updateModel);

			// Assert
			Assert.False(result);
			_mockShelterRepository.Verify(r => r.Update(existingShelter), Times.Once);
			_mockShelterRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
		}
	}
}