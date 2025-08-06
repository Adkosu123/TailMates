using Moq;
using TailMates.Data.Models;
using TailMates.Data.Models.Enums;
using TailMates.Data.Repositories.Interfaces;
using TailMates.Services.Core.Services;
using TailMates.Web.ViewModels.AdoptionApplication;
using Xunit;

namespace TailMates.Services.Core.Tests.AdoptionApplicationServiceTests
{
	public class AdoptionApplicationServiceTests
	{
		private readonly Mock<IAdoptionApplicationRepository> _mockAdoptionApplicationRepository;
		private readonly Mock<IPetRepository> _mockPetRepository;
		private readonly AdoptionApplicationService _adoptionApplicationService;

		public AdoptionApplicationServiceTests()
		{
			_mockAdoptionApplicationRepository = new Mock<IAdoptionApplicationRepository>();
			_mockPetRepository = new Mock<IPetRepository>();
			_adoptionApplicationService = new AdoptionApplicationService(
				_mockAdoptionApplicationRepository.Object,
				_mockPetRepository.Object);
		}

		[Fact]
		public async Task GetAdoptionApplicationViewModelAsync_ShouldReturnViewModel_WhenPetExists()
		{
			// Arrange
			var petId = 1;
			var pet = new Pet
			{
				Id = petId,
				Name = "Buddy",
				Age = 3,
				Description = "Friendly dog.",
				ImageUrl = "buddy.jpg",
				Species = new Species { Name = "Dog" },
				Breed = new Breed { Name = "Golden Retriever" }
			};
			_mockPetRepository.Setup(r => r.GetPetByIdWithDetailsAsync(petId)).ReturnsAsync(pet);

			// Act
			var result = await _adoptionApplicationService.GetAdoptionApplicationViewModelAsync(petId);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(petId, result.PetId);
			Assert.Equal("Buddy", result.PetName);
			Assert.Equal("Dog", result.PetSpecies);
			Assert.Equal("Golden Retriever", result.PetBreed);
			Assert.Equal(3, result.PetAge);
			Assert.Equal("buddy.jpg", result.PetImageUrl);
			Assert.Equal("Friendly dog.", result.PetDescription);
			_mockPetRepository.Verify(r => r.GetPetByIdWithDetailsAsync(petId), Times.Once);
		}

		[Fact]
		public async Task GetAdoptionApplicationViewModelAsync_ShouldReturnNull_WhenPetDoesNotExist()
		{
			// Arrange
			var petId = 99;
			_mockPetRepository.Setup(r => r.GetPetByIdWithDetailsAsync(petId)).ReturnsAsync((Pet)null);

			// Act
			var result = await _adoptionApplicationService.GetAdoptionApplicationViewModelAsync(petId);

			// Assert
			Assert.Null(result);
			_mockPetRepository.Verify(r => r.GetPetByIdWithDetailsAsync(petId), Times.Once);
		}

		[Fact]
		public async Task CreateAdoptionApplicationAsync_ShouldReturnTrue_WhenApplicationIsCreatedSuccessfully()
		{
			// Arrange
			var viewModel = new AdoptionApplicationCreateViewModel
			{
				PetId = 1,
				ApplicantNotes = "I want to adopt this pet."
			};
			var applicantId = "user123";
			var availablePet = new Pet { Id = 1, Name = "Available Pet", IsAdopted = false };

			_mockPetRepository.Setup(r => r.GetAvailablePetByIdAsync(viewModel.PetId)).ReturnsAsync(availablePet);
			_mockAdoptionApplicationRepository.Setup(r => r.HasPendingApplicationForPetAndApplicantAsync(viewModel.PetId, applicantId)).ReturnsAsync(false);
			_mockAdoptionApplicationRepository.Setup(r => r.AddAsync(It.IsAny<AdoptionApplication>())).Returns(Task.CompletedTask);
			_mockAdoptionApplicationRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1); // Simulate 1 change saved

			// Act
			var result = await _adoptionApplicationService.CreateAdoptionApplicationAsync(viewModel, applicantId);

			// Assert
			Assert.True(result);
			_mockPetRepository.Verify(r => r.GetAvailablePetByIdAsync(viewModel.PetId), Times.Once);
			_mockAdoptionApplicationRepository.Verify(r => r.HasPendingApplicationForPetAndApplicantAsync(viewModel.PetId, applicantId), Times.Once);
			_mockAdoptionApplicationRepository.Verify(r => r.AddAsync(It.IsAny<AdoptionApplication>()), Times.Once);
			_mockAdoptionApplicationRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
		}

		[Fact]
		public async Task CreateAdoptionApplicationAsync_ShouldReturnFalse_WhenPetDoesNotExist()
		{
			// Arrange
			var viewModel = new AdoptionApplicationCreateViewModel { PetId = 99 };
			var applicantId = "user123";

			_mockPetRepository.Setup(r => r.GetAvailablePetByIdAsync(viewModel.PetId)).ReturnsAsync((Pet)null);

			// Act
			var result = await _adoptionApplicationService.CreateAdoptionApplicationAsync(viewModel, applicantId);

			// Assert
			Assert.False(result);
			_mockPetRepository.Verify(r => r.GetAvailablePetByIdAsync(viewModel.PetId), Times.Once);
			_mockAdoptionApplicationRepository.Verify(r => r.HasPendingApplicationForPetAndApplicantAsync(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
			_mockAdoptionApplicationRepository.Verify(r => r.AddAsync(It.IsAny<AdoptionApplication>()), Times.Never);
			_mockAdoptionApplicationRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
		}

		[Fact]
		public async Task CreateAdoptionApplicationAsync_ShouldReturnFalse_WhenPetIsNotAvailable()
		{
			// Arrange
			var viewModel = new AdoptionApplicationCreateViewModel { PetId = 1 };
			var applicantId = "user123";

			_mockPetRepository.Setup(r => r.GetAvailablePetByIdAsync(viewModel.PetId)).ReturnsAsync((Pet)null);

			// Act
			var result = await _adoptionApplicationService.CreateAdoptionApplicationAsync(viewModel, applicantId);

			// Assert
			Assert.False(result);
			_mockPetRepository.Verify(r => r.GetAvailablePetByIdAsync(viewModel.PetId), Times.Once);
			// Verify that subsequent repository calls (HasPendingApplicationForPetAndApplicantAsync, AddAsync, SaveChangesAsync) are NOT made
			_mockAdoptionApplicationRepository.Verify(r => r.HasPendingApplicationForPetAndApplicantAsync(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
			_mockAdoptionApplicationRepository.Verify(r => r.AddAsync(It.IsAny<AdoptionApplication>()), Times.Never);
			_mockAdoptionApplicationRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
		}

		[Fact]
		public async Task CreateAdoptionApplicationAsync_ShouldReturnFalse_WhenPendingApplicationAlreadyExists()
		{
			// Arrange
			var viewModel = new AdoptionApplicationCreateViewModel { PetId = 1 };
			var applicantId = "user123";
			var availablePet = new Pet { Id = 1, Name = "Available Pet", IsAdopted = false };

			_mockPetRepository.Setup(r => r.GetAvailablePetByIdAsync(viewModel.PetId)).ReturnsAsync(availablePet);
			_mockAdoptionApplicationRepository.Setup(r => r.HasPendingApplicationForPetAndApplicantAsync(viewModel.PetId, applicantId)).ReturnsAsync(true); // Simulate existing pending application

			// Act
			var result = await _adoptionApplicationService.CreateAdoptionApplicationAsync(viewModel, applicantId);

			// Assert
			Assert.False(result);
			_mockPetRepository.Verify(r => r.GetAvailablePetByIdAsync(viewModel.PetId), Times.Once);
			_mockAdoptionApplicationRepository.Verify(r => r.HasPendingApplicationForPetAndApplicantAsync(viewModel.PetId, applicantId), Times.Once);
			_mockAdoptionApplicationRepository.Verify(r => r.AddAsync(It.IsAny<AdoptionApplication>()), Times.Never);
			_mockAdoptionApplicationRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
		}

		[Fact]
		public async Task CreateAdoptionApplicationAsync_ShouldReturnFalse_WhenSaveChangesAsyncFails()
		{
			// Arrange
			var viewModel = new AdoptionApplicationCreateViewModel { PetId = 1 };
			var applicantId = "user123";
			var availablePet = new Pet { Id = 1, Name = "Available Pet", IsAdopted = false };

			_mockPetRepository.Setup(r => r.GetAvailablePetByIdAsync(viewModel.PetId)).ReturnsAsync(availablePet);
			_mockAdoptionApplicationRepository.Setup(r => r.HasPendingApplicationForPetAndApplicantAsync(viewModel.PetId, applicantId)).ReturnsAsync(false);
			_mockAdoptionApplicationRepository.Setup(r => r.AddAsync(It.IsAny<AdoptionApplication>())).Returns(Task.CompletedTask);
			_mockAdoptionApplicationRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(0); // Simulate 0 changes saved

			// Act
			var result = await _adoptionApplicationService.CreateAdoptionApplicationAsync(viewModel, applicantId);

			// Assert
			Assert.False(result);
			_mockPetRepository.Verify(r => r.GetAvailablePetByIdAsync(viewModel.PetId), Times.Once);
			_mockAdoptionApplicationRepository.Verify(r => r.HasPendingApplicationForPetAndApplicantAsync(viewModel.PetId, applicantId), Times.Once);
			_mockAdoptionApplicationRepository.Verify(r => r.AddAsync(It.IsAny<AdoptionApplication>()), Times.Once);
			_mockAdoptionApplicationRepository.Verify(r => r.SaveChangesAsync(), Times.Once); // Still verify save was attempted
		}
	}
}