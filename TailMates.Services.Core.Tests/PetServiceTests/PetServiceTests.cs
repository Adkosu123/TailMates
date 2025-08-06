using Microsoft.AspNetCore.Identity;
using MockQueryable.Moq;
using Moq;
using TailMates.Data.Models;
using TailMates.Data.Models.Enums;
using TailMates.Data.Repositories.Interfaces;
using TailMates.Services.Core.Services;
using TailMates.Web.ViewModels.Pet;
using Xunit;
namespace TailMates.Services.Core.Tests
{
	public class PetServiceTests
	{
		private readonly Mock<IPetRepository> _mockPetRepository;
		private readonly Mock<ISpeciesRepository> _mockSpeciesRepository;
		private readonly Mock<IBreedRepository> _mockBreedRepository;
		private readonly Mock<IShelterRepository> _mockShelterRepository;
		private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
		private readonly PetService _petService;

		public PetServiceTests()
		{
			_mockPetRepository = new Mock<IPetRepository>();
			_mockSpeciesRepository = new Mock<ISpeciesRepository>();
			_mockBreedRepository = new Mock<IBreedRepository>();
			_mockShelterRepository = new Mock<IShelterRepository>();
			var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
			_mockUserManager = new Mock<UserManager<ApplicationUser>>(
				userStoreMock.Object, null, null, null, null, null, null, null, null);

			_petService = new PetService(
				_mockPetRepository.Object,
				_mockSpeciesRepository.Object,
				_mockBreedRepository.Object,
				_mockShelterRepository.Object,
				_mockUserManager.Object);
		}

		[Fact]
		public async Task AddPetAsync_ShouldReturnTrue_WhenAdminAddsPet()
		{
			var newPetVm = new PetCreateViewModel
			{
				Name = "Buddy",
				Age = 3,
				Description = "Friendly dog",
				ImageUrl = "url",
				Gender = PetGender.Male,
				SpeciesId = 1,
				BreedId = 1,
				ShelterId = 1
			};
			_mockPetRepository.Setup(r => r.AddAsync(It.IsAny<Pet>())).Returns(Task.CompletedTask);
			_mockPetRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

			// Act
			var result = await _petService.AddPetAsync(newPetVm, "adminId", true);

			// Assert
			Assert.True(result);
			_mockPetRepository.Verify(r => r.AddAsync(It.IsAny<Pet>()), Times.Once);
			_mockPetRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
		}

		[Fact]
		public async Task AddPetAsync_ShouldReturnTrue_WhenManagerAddsPetToAssignedShelter()
		{
			// Arrange
			var managerId = "manager123";
			var assignedShelterId = 1;
			var newPetVm = new PetCreateViewModel
			{
				Name = "Buddy",
				Age = 3,
				Description = "Friendly dog",
				ImageUrl = "url",
				Gender = PetGender.Male,
				SpeciesId = 1,
				BreedId = 1,
				ShelterId = assignedShelterId
			};
			var managerUser = new ApplicationUser { Id = managerId, ManagedShelterId = assignedShelterId };

			_mockUserManager.Setup(um => um.FindByIdAsync(managerId)).ReturnsAsync(managerUser);
			_mockPetRepository.Setup(r => r.AddAsync(It.IsAny<Pet>())).Returns(Task.CompletedTask);
			_mockPetRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

			// Act
			var result = await _petService.AddPetAsync(newPetVm, managerId, false);

			// Assert
			Assert.True(result);
			_mockPetRepository.Verify(r => r.AddAsync(It.IsAny<Pet>()), Times.Once);
			_mockPetRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
		}

		[Fact]
		public async Task AddPetAsync_ShouldReturnFalse_WhenManagerAddsPetToUnassignedShelter()
		{
			// Arrange
			var managerId = "manager123";
			var assignedShelterId = 1;
			var unassignedShelterId = 2;
			var newPetVm = new PetCreateViewModel
			{
				Name = "Buddy",
				Age = 3,
				Description = "Friendly dog",
				ImageUrl = "url",
				Gender = PetGender.Male,
				SpeciesId = 1,
				BreedId = 1,
				ShelterId = unassignedShelterId // Wrong shelter
			};
			var managerUser = new ApplicationUser { Id = managerId, ManagedShelterId = assignedShelterId };

			_mockUserManager.Setup(um => um.FindByIdAsync(managerId)).ReturnsAsync(managerUser);

			// Act
			var result = await _petService.AddPetAsync(newPetVm, managerId, false);

			// Assert
			Assert.False(result);
			_mockPetRepository.Verify(r => r.AddAsync(It.IsAny<Pet>()), Times.Never); // Should not try to add
			_mockPetRepository.Verify(r => r.SaveChangesAsync(), Times.Never); // Should not try to save
		}

		[Fact]
		public async Task AddPetAsync_ShouldReturnFalse_WhenSaveChangesAsyncFails()
		{
			// Arrange
			var newPetVm = new PetCreateViewModel
			{
				Name = "Buddy",
				Age = 3,
				Description = "Friendly dog",
				ImageUrl = "url",
				Gender = PetGender.Male,
				SpeciesId = 1,
				BreedId = 1,
				ShelterId = 1
			};
			_mockPetRepository.Setup(r => r.AddAsync(It.IsAny<Pet>())).Returns(Task.CompletedTask);
			_mockPetRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(0); // Simulate failure

			// Act
			var result = await _petService.AddPetAsync(newPetVm, "adminId", true);

			// Assert
			Assert.False(result);
			_mockPetRepository.Verify(r => r.AddAsync(It.IsAny<Pet>()), Times.Once);
			_mockPetRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
		}

		[Fact]
		public async Task GetFilteredPetsAsync_ShouldReturnAllNonDeletedAndNonAdoptedPets_WhenNoFiltersApplied()
		{
			// Arrange
			var pets = new List<Pet>
			{
				new Pet { Id = 1, Name = "Pet1", IsDeleted = false, IsAdopted = false, Species = new Species { Name = "Dog" }, Breed = new Breed { Name = "Lab" }, Shelter = new Shelter { Name = "ShelterA" } },
				new Pet { Id = 2, Name = "Pet2", IsDeleted = false, IsAdopted = false, Species = new Species { Name = "Cat" }, Breed = new Breed { Name = "Siamese" }, Shelter = new Shelter { Name = "ShelterB" } },
				new Pet { Id = 3, Name = "DeletedPet", IsDeleted = true, IsAdopted = false, Species = new Species { Name = "Bird" }, Breed = new Breed { Name = "Parrot" }, Shelter = new Shelter { Name = "ShelterC" } },
				new Pet { Id = 4, Name = "AdoptedPet", IsDeleted = false, IsAdopted = true, Species = new Species { Name = "Fish" }, Breed = new Breed { Name = "Goldfish" }, Shelter = new Shelter { Name = "ShelterD" } }
			};

			// Use MockQueryable.Moq to create a mock IQueryable that supports async operations
			var mockQueryablePets = pets.AsQueryable().BuildMock();

			_mockPetRepository.Setup(r => r.AllAsNoTracking())
				.Returns(mockQueryablePets);

			_mockPetRepository.Setup(r => r.AllAsNoTracking())
				.Returns(pets.AsQueryable().BuildMock()); // Start with the base queryable

			var petsWithIncludes = pets.AsQueryable().BuildMock();


			// Act
			var result = await _petService.GetFilteredPetsAsync(new PetFilterViewModel(), 1, 10);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(2, result.Count); // Only Pet1 and Pet2 should be returned
			Assert.Contains(result, p => p.Name == "Pet1");
			Assert.Contains(result, p => p.Name == "Pet2");
			Assert.DoesNotContain(result, p => p.Name == "DeletedPet");
			Assert.DoesNotContain(result, p => p.Name == "AdoptedPet");
		}

		[Fact]
		public async Task GetFilteredPetsAsync_ShouldFilterBySearchTerm()
		{
			// Arrange
			var pets = new List<Pet>
			{
				new Pet { Id = 1, Name = "Buddy", IsDeleted = false, IsAdopted = false, Species = new Species { Name = "Dog" }, Breed = new Breed { Name = "Lab" }, Shelter = new Shelter { Name = "ShelterA" }, Description = "A friendly dog" },
				new Pet { Id = 2, Name = "Whiskers", IsDeleted = false, IsAdopted = false, Species = new Species { Name = "Cat" }, Breed = new Breed { Name = "Siamese" }, Shelter = new Shelter { Name = "ShelterB" }, Description = "A playful cat" }
			};

			// Use MockQueryable.Moq
			_mockPetRepository.Setup(r => r.AllAsNoTracking())
							  .Returns(pets.AsQueryable().BuildMock());

			var filters = new PetFilterViewModel { SearchTerm = "dog" };

			// Act
			var result = await _petService.GetFilteredPetsAsync(filters, 1, 10);

			// Assert
			Assert.NotNull(result);
			Assert.Single(result);
			Assert.Contains(result, p => p.Name == "Buddy");
			Assert.Equal(1, result.TotalCount);
			Assert.Equal(1, result.PageIndex);
			Assert.Equal(10, result.PageSize);
		}

		[Fact]
		public async Task UpdatePetAsync_ShouldReturnTrue_WhenPetExistsAndIsUpdated()
		{
			// Arrange
			var existingPet = new Pet { Id = 1, Name = "Old Name", Age = 5, SpeciesId = 1, BreedId = 1, ShelterId = 1 };
			var updatedPetVm = new PetEditViewModel
			{
				Id = 1,
				Name = "New Name",
				Age = 6,
				Description = "New Desc",
				ImageUrl = "new_url",
				Gender = PetGender.Female,
				SpeciesId = 2,
				BreedId = 2,
				ShelterId = 2
			};

			_mockPetRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingPet);
			_mockPetRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

			// Act
			var result = await _petService.UpdatePetAsync(updatedPetVm);

			// Assert
			Assert.True(result);
			Assert.Equal("New Name", existingPet.Name); // Verify the entity was updated
			Assert.Equal(6, existingPet.Age);
			_mockPetRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
		}

		[Fact]
		public async Task UpdatePetAsync_ShouldReturnFalse_WhenPetDoesNotExist()
		{
			// Arrange
			var updatedPetVm = new PetEditViewModel { Id = 99, Name = "NonExistent" };
			_mockPetRepository.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Pet)null);

			// Act
			var result = await _petService.UpdatePetAsync(updatedPetVm);

			// Assert
			Assert.False(result);
			_mockPetRepository.Verify(r => r.SaveChangesAsync(), Times.Never); // Should not try to save
		}

		[Fact]
		public async Task RemovePetAsync_ShouldReturnTrue_WhenPetExists()
		{
			// Arrange
			var petToRemove = new Pet { Id = 1, Name = "PetToDelete", IsDeleted = false };
			_mockPetRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(petToRemove);
			_mockPetRepository.Setup(r => r.Update(It.IsAny<Pet>())); // No return needed for void
			_mockPetRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

			// Act
			var result = await _petService.RemovePetAsync(1);

			// Assert
			Assert.True(result);
			Assert.True(petToRemove.IsDeleted); // Verify soft delete flag is set
			_mockPetRepository.Verify(r => r.Update(petToRemove), Times.Once);
			_mockPetRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
		}

		[Fact]
		public async Task RemovePetAsync_ShouldReturnFalse_WhenPetDoesNotExist()
		{
			// Arrange
			_mockPetRepository.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Pet)null);

			// Act
			var result = await _petService.RemovePetAsync(99);

			// Assert
			Assert.False(result);
			_mockPetRepository.Verify(r => r.Update(It.IsAny<Pet>()), Times.Never);
			_mockPetRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
		}

		[Fact]
		public async Task GetPetDetailsForUserAsync_ShouldReturnDetails_WhenPetExistsAndNotAdopted()
		{
			// Arrange
			var pet = new Pet
			{
				Id = 1,
				Name = "Fluffy",
				IsAdopted = false,
				Species = new Species { Name = "Cat" }, // Ensure Species is initialized for Include
				Breed = new Breed { Name = "Persian" },   // Ensure Breed is initialized for Include
				Shelter = new Shelter { Name = "Cat Haven", Address = "123 Main St" } // Ensure Shelter is initialized for Include
			};

			// Crucial: Use BuildMock() for AllAsNoTracking to simulate async IQueryable
			_mockPetRepository.Setup(r => r.AllAsNoTracking())
				.Returns(new List<Pet> { pet }.AsQueryable().BuildMock());

			// Act
			var result = await _petService.GetPetDetailsForUserAsync(1);

			// Assert
			Assert.NotNull(result);
			Assert.Equal("Fluffy", result.Name);
			Assert.False(result.IsAdopted);
			Assert.Equal("Cat Haven", result.ShelterName);
			// Add more assertions based on PetDetailsViewModel properties
			Assert.Equal("Cat", result.SpeciesName);
			Assert.Equal("Persian", result.BreedName);
			Assert.Equal("123 Main St", result.ShelterAddress);
		}

		[Fact]
		public async Task GetPetDetailsForUserAsync_ShouldReturnNull_WhenPetDoesNotExist()
		{
			_mockPetRepository.Setup(r => r.AllAsNoTracking())
				.Returns(new List<Pet>().AsQueryable().BuildMock());

			// Act
			var result = await _petService.GetPetDetailsForUserAsync(99);

			// Assert
			Assert.Null(result);
		}


		[Fact]
		public async Task GetPetByIdWithAdoptionDetailsAsync_ShouldReturnPetWithDetails()
		{
			// Arrange
			var pet = new Pet
			{
				Id = 1,
				Name = "DetailedPet",
				IsAdopted = false,
				Species = new Species { Id = 1, Name = "Dog" },
				Breed = new Breed { Id = 1, Name = "Poodle" },
				Shelter = new Shelter { Id = 1, Name = "Best Shelter" },
				AdoptionApplications = new List<AdoptionApplication> { new AdoptionApplication() }
			};

			_mockPetRepository.Setup(r => r.AllAsNoTracking())
				.Returns(new List<Pet> { pet }.AsQueryable().BuildMock());

			// Act
			var result = await _petService.GetPetByIdWithAdoptionDetailsAsync(1);

			// Assert
			Assert.NotNull(result);
			Assert.Equal("DetailedPet", result.Name);
			Assert.NotNull(result.Species);
			Assert.NotNull(result.Breed);
			Assert.NotNull(result.Shelter);
			Assert.NotEmpty(result.AdoptionApplications);
		}
		[Fact]
		public async Task GetPetFormDropdownsAsync_ShouldFilterSheltersForManager()
		{
			// Arrange
			var managerId = "managerId1";
			var assignedShelterId = 10;
			var managerUser = new ApplicationUser { Id = managerId, ManagedShelterId = assignedShelterId };

			var species = new List<Species> { new Species { Id = 1, Name = "Dog" } };
			var breeds = new List<Breed> { new Breed { Id = 1, Name = "Labrador" } };
			var allShelters = new List<Shelter>
			{
				new Shelter { Id = 10, Name = "Manager's Shelter" },
				new Shelter { Id = 20, Name = "Other Shelter" }
			};

			_mockUserManager.Setup(um => um.FindByIdAsync(managerId)).ReturnsAsync(managerUser);
			_mockSpeciesRepository.Setup(r => r.GetAllAsync()).Returns(Task.FromResult<IEnumerable<Species>>(species));
			_mockBreedRepository.Setup(r => r.GetAllAsync()).Returns(Task.FromResult<IEnumerable<Breed>>(breeds));
			_mockShelterRepository.Setup(r => r.GetAllAsync()).Returns(Task.FromResult<IEnumerable<Shelter>>(allShelters));

			// Act
			var result = await _petService.GetPetFormDropdownsAsync(managerId, true); // isManager = true

			// Assert
			Assert.NotNull(result.ShelterList);
			Assert.Single(result.ShelterList.Items);
			Assert.Equal("Manager's Shelter", result.ShelterList.First().Text);
			Assert.Equal(assignedShelterId.ToString(), result.ShelterList.First().Value);
		}

		// --- New Tests for the provided methods ---

		[Fact]
		public async Task GetShelterByIdAsync_ShouldReturnShelter_WhenFound()
		{
			// Arrange
			var shelterId = 5;
			var expectedShelter = new Shelter { Id = shelterId, Name = "Test Shelter" };
			_mockShelterRepository.Setup(r => r.GetByIdAsync(shelterId)).ReturnsAsync(expectedShelter);

			// Act
			var result = await _petService.GetShelterByIdAsync(shelterId);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(shelterId, result.Id);
			Assert.Equal("Test Shelter", result.Name);
			_mockShelterRepository.Verify(r => r.GetByIdAsync(shelterId), Times.Once);
		}

		[Fact]
		public async Task GetShelterByIdAsync_ShouldReturnNull_WhenNotFound()
		{
			// Arrange
			var shelterId = 99;
			_mockShelterRepository.Setup(r => r.GetByIdAsync(shelterId)).ReturnsAsync((Shelter)null);

			// Act
			var result = await _petService.GetShelterByIdAsync(shelterId);

			// Assert
			Assert.Null(result);
			_mockShelterRepository.Verify(r => r.GetByIdAsync(shelterId), Times.Once);
		}

		[Fact]
		public async Task GetSheltersAsSelectListAsync_FromShelterRepository_ShouldReturnEmptyList_WhenNoShelters()
		{
			// Arrange
			_mockShelterRepository.Setup(r => r.GetAllAsync()).Returns(Task.FromResult<IEnumerable<Shelter>>(new List<Shelter>()));

			// Act
			var result = await _petService.GetSheltersAsSelectListAsync(); // This calls the first method

			// Assert
			Assert.NotNull(result);
			Assert.Empty(result.Items);
		}

		[Fact]
		public async Task GetGendersSelectListAsync_ShouldReturnAllGendersWithAllOption_NoSelection()
		{
			// Arrange - No mock setup needed as it's purely in-memory enum processing

			// Act
			var result = await _petService.GetGendersSelectListAsync(null);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(Enum.GetValues(typeof(PetGender)).Length + 1, result.Count()); // All Genders + enum values
			Assert.Equal("All Genders", result.First().Text);
			Assert.True(result.First().Selected);
			Assert.Contains(result, i => i.Text == "Male");
			Assert.Contains(result, i => i.Text == "Female");
			Assert.Contains(result, i => i.Text == "Unknown"); // Assuming PetGender has Male, Female, Unknown
		}

		[Fact]
		public async Task GetGendersSelectListAsync_ShouldReturnAllGendersWithSelectedOption()
		{
			// Arrange - No mock setup needed

			// Act
			var result = await _petService.GetGendersSelectListAsync("Female");

			// Assert
			Assert.NotNull(result);
			Assert.Equal(Enum.GetValues(typeof(PetGender)).Length + 1, result.Count());
			Assert.Equal("All Genders", result.First().Text);
			Assert.False(result.First().Selected);
			Assert.True(result.Any(i => i.Value == "Female" && i.Selected));
		}

		[Fact]
		public async Task GetBreedsSelectListForSpeciesAsync_ShouldReturnAllBreedsForSpeciesWithAllOption_NoSelection()
		{
			// Arrange
			var speciesId = 1;
			var breedsLookup = new List<Breed>
			{
				new Breed { Id = 1, Name = "Labrador", SpeciesId = speciesId },
				new Breed { Id = 2, Name = "Poodle", SpeciesId = speciesId }
			};
			_mockPetRepository.Setup(r => r.GetBreedsForSpeciesLookupAsync(speciesId)).Returns(Task.FromResult<IEnumerable<Breed>>(breedsLookup));

			// Act
			var result = await _petService.GetBreedsSelectListForSpeciesAsync(speciesId, null);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(3, result.Count()); // All Breeds + Labrador + Poodle
			Assert.Equal("All Breeds", result.First().Text);
			Assert.True(result.First().Selected);
			Assert.Equal("Labrador", result.Skip(1).First().Text); // Ordered alphabetically
			Assert.Equal("Poodle", result.Last().Text);
		}

		[Fact]
		public async Task GetBreedsSelectListForSpeciesAsync_ShouldReturnAllBreedsForSpeciesWithSelectedOption()
		{
			// Arrange
			var speciesId = 1;
			var selectedBreedId = 2;
			var breedsLookup = new List<Breed>
			{
				new Breed { Id = 1, Name = "Labrador", SpeciesId = speciesId },
				new Breed { Id = 2, Name = "Poodle", SpeciesId = speciesId }
			};
			_mockPetRepository.Setup(r => r.GetBreedsForSpeciesLookupAsync(speciesId)).Returns(Task.FromResult<IEnumerable<Breed>>(breedsLookup));

			// Act
			var result = await _petService.GetBreedsSelectListForSpeciesAsync(speciesId, selectedBreedId);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(3, result.Count());
			Assert.Equal("All Breeds", result.First().Text);
			Assert.False(result.First().Selected); // All Breeds should not be selected
			Assert.True(result.Any(i => i.Value == selectedBreedId.ToString() && i.Selected)); // Poodle should be selected
		}

		[Fact]
		public async Task GetBreedsSelectListForSpeciesAsync_ShouldReturnOnlyAllBreeds_WhenInvalidSpeciesId()
		{
			// Arrange
			var speciesId = 0; // Invalid species ID
							   // No setup for GetBreedsForSpeciesLookupAsync needed as it shouldn't be called

			// Act
			var result = await _petService.GetBreedsSelectListForSpeciesAsync(speciesId, null);

			// Assert
			Assert.NotNull(result);
			Assert.Single(result); // Only "All Breeds" option
			Assert.Equal("All Breeds", result.First().Text);
			Assert.True(result.First().Selected);
			_mockPetRepository.Verify(r => r.GetBreedsForSpeciesLookupAsync(It.IsAny<int>()), Times.Never); // Verify it wasn't called
		}
		[Fact]
		public async Task GetFilteredPetsAsync_ShouldFilterBySpeciesId()
		{
			// Arrange
			var pets = new List<Pet>
			{
				new Pet { Id = 1, Name = "Doggo", SpeciesId = 1, BreedId = 1, IsDeleted = false, IsAdopted = false, Species = new Species { Name = "Dog" }, Breed = new Breed { Name = "Lab" }, Shelter = new Shelter { Name = "ShelterA" } },
				new Pet { Id = 2, Name = "Kitty", SpeciesId = 2, BreedId = 2, IsDeleted = false, IsAdopted = false, Species = new Species { Name = "Cat" }, Breed = new Breed { Name = "Siamese" }, Shelter = new Shelter { Name = "ShelterB" } },
				new Pet { Id = 3, Name = "Another Dog", SpeciesId = 1, BreedId = 3, IsDeleted = false, IsAdopted = false, Species = new Species { Name = "Dog" }, Breed = new Breed { Name = "Poodle" }, Shelter = new Shelter { Name = "ShelterC" } }
			};

			_mockPetRepository.Setup(r => r.AllAsNoTracking())
							  .Returns(pets.AsQueryable().BuildMock());

			var filters = new PetFilterViewModel { SpeciesId = 1 }; // Filter for dogs

			// Act
			var result = await _petService.GetFilteredPetsAsync(filters, 1, 10);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(2, result.Count); // Should get Doggo and Another Dog
			Assert.Contains(result, p => p.Name == "Doggo");
			Assert.Contains(result, p => p.Name == "Another Dog");
			Assert.DoesNotContain(result, p => p.Name == "Kitty");
			Assert.Equal(2, result.TotalCount);
		}

		[Fact]
		public async Task GetFilteredPetsAsync_ShouldFilterByBreedId()
		{
			// Arrange
			var pets = new List<Pet>
			{
				new Pet { Id = 1, Name = "Labrador", SpeciesId = 1, BreedId = 1, IsDeleted = false, IsAdopted = false, Species = new Species { Name = "Dog" }, Breed = new Breed { Name = "Labrador" }, Shelter = new Shelter { Name = "ShelterA" } },
				new Pet { Id = 2, Name = "Poodle", SpeciesId = 1, BreedId = 2, IsDeleted = false, IsAdopted = false, Species = new Species { Name = "Dog" }, Breed = new Breed { Name = "Poodle" }, Shelter = new Shelter { Name = "ShelterB" } },
				new Pet { Id = 3, Name = "Siamese", SpeciesId = 2, BreedId = 3, IsDeleted = false, IsAdopted = false, Species = new Species { Name = "Cat" }, Breed = new Breed { Name = "Siamese" }, Shelter = new Shelter { Name = "ShelterC" } }
			};

			_mockPetRepository.Setup(r => r.AllAsNoTracking())
							  .Returns(pets.AsQueryable().BuildMock());

			var filters = new PetFilterViewModel { BreedId = 2 }; // Filter for Poodle

			// Act
			var result = await _petService.GetFilteredPetsAsync(filters, 1, 10);

			// Assert
			Assert.NotNull(result);
			Assert.Single(result); // Should get only Poodle
			Assert.Contains(result, p => p.Name == "Poodle");
			Assert.Equal(1, result.TotalCount);
		}

		[Fact]
		public async Task GetFilteredPetsAsync_ShouldFilterByGender()
		{
			// Arrange
			var pets = new List<Pet>
			{
				new Pet { Id = 1, Name = "Male Dog", Gender = PetGender.Male, IsDeleted = false, IsAdopted = false, Species = new Species { Name = "Dog" }, Breed = new Breed { Name = "Lab" }, Shelter = new Shelter { Name = "ShelterA" } },
				new Pet { Id = 2, Name = "Female Cat", Gender = PetGender.Female, IsDeleted = false, IsAdopted = false, Species = new Species { Name = "Cat" }, Breed = new Breed { Name = "Siamese" }, Shelter = new Shelter { Name = "ShelterB" } },
				new Pet { Id = 3, Name = "Another Male Dog", Gender = PetGender.Male, IsDeleted = false, IsAdopted = false, Species = new Species { Name = "Dog" }, Breed = new Breed { Name = "Poodle" }, Shelter = new Shelter { Name = "ShelterC" } }
			};

			_mockPetRepository.Setup(r => r.AllAsNoTracking())
							  .Returns(pets.AsQueryable().BuildMock());

			var filters = new PetFilterViewModel { Gender = "Male" }; // Filter for Male pets

			// Act
			var result = await _petService.GetFilteredPetsAsync(filters, 1, 10);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(2, result.Count); // Should get Male Dog and Another Male Dog
			Assert.Contains(result, p => p.Name == "Male Dog");
			Assert.Contains(result, p => p.Name == "Another Male Dog");
			Assert.DoesNotContain(result, p => p.Name == "Female Cat");
			Assert.Equal(2, result.TotalCount);
		}

		[Fact]
		public async Task GetFilteredPetsAsync_ShouldFilterByShelterId()
		{
			// Arrange
			var pets = new List<Pet>
			{
				new Pet { Id = 1, Name = "Shelter A Pet", ShelterId = 1, IsDeleted = false, IsAdopted = false, Species = new Species { Name = "Dog" }, Breed = new Breed { Name = "Lab" }, Shelter = new Shelter { Name = "ShelterA" } },
				new Pet { Id = 2, Name = "Shelter B Pet", ShelterId = 2, IsDeleted = false, IsAdopted = false, Species = new Species { Name = "Cat" }, Breed = new Breed { Name = "Siamese" }, Shelter = new Shelter { Name = "ShelterB" } },
				new Pet { Id = 3, Name = "Another Shelter A Pet", ShelterId = 1, IsDeleted = false, IsAdopted = false, Species = new Species { Name = "Dog" }, Breed = new Breed { Name = "Poodle" }, Shelter = new Shelter { Name = "ShelterA" } }
			};

			_mockPetRepository.Setup(r => r.AllAsNoTracking())
							  .Returns(pets.AsQueryable().BuildMock());

			var filters = new PetFilterViewModel { ShelterId = 1 }; // Filter for Shelter A

			// Act
			var result = await _petService.GetFilteredPetsAsync(filters, 1, 10);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(2, result.Count); // Should get Shelter A Pet and Another Shelter A Pet
			Assert.Contains(result, p => p.Name == "Shelter A Pet");
			Assert.Contains(result, p => p.Name == "Another Shelter A Pet");
			Assert.DoesNotContain(result, p => p.Name == "Shelter B Pet");
			Assert.Equal(2, result.TotalCount);
		}

		[Fact]
		public async Task GetAllPetsAsync_ShouldReturnAllPetsWithDetails()
		{
			// Arrange
			var pets = new List<Pet>
			{
				new Pet { Id = 1, Name = "Pet1", Age = 2, Gender = PetGender.Male, IsDeleted = false, IsAdopted = false, Species = new Species { Name = "Dog" }, Breed = new Breed { Name = "Lab" }, Shelter = new Shelter { Name = "ShelterA", Address = "123 Main" } },
				new Pet { Id = 2, Name = "Pet2", Age = 3, Gender = PetGender.Female, IsDeleted = false, IsAdopted = false, Species = new Species { Name = "Cat" }, Breed = new Breed { Name = "Siamese" }, Shelter = new Shelter { Name = "ShelterB", Address = "456 Oak" } }
			};

			// Mock the GetAllPetsWithDetails to return an IQueryable that supports async operations
			_mockPetRepository.Setup(r => r.GetAllPetsWithDetails())
							  .Returns(pets.AsQueryable().BuildMock());

			// Act
			var result = await _petService.GetAllPetsAsync();

			// Assert
			Assert.NotNull(result);
			Assert.Equal(2, result.Count());
			var pet1 = result.First(p => p.Id == 1);
			Assert.Equal("Pet1", pet1.Name);
			Assert.Equal("Dog", pet1.SpeciesName);
			Assert.Equal("Lab", pet1.BreedName);
			Assert.Equal("ShelterA", pet1.ShelterName);
			Assert.Equal("Male", pet1.Gender);
		}

		[Fact]
		public async Task GetAllPetsAsync_ShouldReturnEmptyList_WhenNoPetsExist()
		{
			// Arrange
			_mockPetRepository.Setup(r => r.GetAllPetsWithDetails())
							  .Returns(new List<Pet>().AsQueryable().BuildMock());

			// Act
			var result = await _petService.GetAllPetsAsync();

			// Assert
			Assert.NotNull(result);
			Assert.Empty(result);
		}

		[Fact]
		public async Task AddPetAsync_ShouldReturnFalse_WhenUserIsNullAndNotAdmin()
		{
			// Arrange
			var newPetVm = new PetCreateViewModel
			{
				Name = "Buddy",
				Age = 3,
				Description = "Friendly dog",
				ImageUrl = "url",
				Gender = PetGender.Male,
				SpeciesId = 1,
				BreedId = 1,
				ShelterId = 1
			};

			// Act
			// currentUserId is null, isAdmin is false
			var result = await _petService.AddPetAsync(newPetVm, null, false);

			// Assert
			Assert.False(result);
			_mockUserManager.Verify(um => um.FindByIdAsync(It.IsAny<string>()), Times.Never); // Should not try to find user
			_mockPetRepository.Verify(r => r.AddAsync(It.IsAny<Pet>()), Times.Never);
			_mockPetRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
		}

		[Fact]
		public async Task AddPetAsync_ShouldReturnFalse_WhenUserManagerFindByIdAsyncReturnsNull()
		{
			// Arrange
			var newPetVm = new PetCreateViewModel
			{
				Name = "Buddy",
				Age = 3,
				Description = "Friendly dog",
				ImageUrl = "url",
				Gender = PetGender.Male,
				SpeciesId = 1,
				BreedId = 1,
				ShelterId = 1
			};
			var currentUserId = "nonExistentUser";

			_mockUserManager.Setup(um => um.FindByIdAsync(currentUserId)).ReturnsAsync((ApplicationUser)null);

			// Act
			var result = await _petService.AddPetAsync(newPetVm, currentUserId, false);

			// Assert
			Assert.False(result);
			_mockUserManager.Verify(um => um.FindByIdAsync(currentUserId), Times.Once);
			_mockPetRepository.Verify(r => r.AddAsync(It.IsAny<Pet>()), Times.Never);
			_mockPetRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
		}

		[Fact]
		public async Task AddPetAsync_ShouldReturnFalse_WhenUserHasNoManagedShelterId()
		{
			// Arrange
			var newPetVm = new PetCreateViewModel
			{
				Name = "Buddy",
				Age = 3,
				Description = "Friendly dog",
				ImageUrl = "url",
				Gender = PetGender.Male,
				SpeciesId = 1,
				BreedId = 1,
				ShelterId = 1
			};
			var currentUserId = "userWithNoShelter";
			var user = new ApplicationUser { Id = currentUserId, ManagedShelterId = null }; // No managed shelter

			_mockUserManager.Setup(um => um.FindByIdAsync(currentUserId)).ReturnsAsync(user);

			// Act
			var result = await _petService.AddPetAsync(newPetVm, currentUserId, false);

			// Assert
			Assert.False(result);
			_mockUserManager.Verify(um => um.FindByIdAsync(currentUserId), Times.Once);
			_mockPetRepository.Verify(r => r.AddAsync(It.IsAny<Pet>()), Times.Never);
			_mockPetRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
		}

		[Fact]
		public async Task UpdatePetAsync_ShouldReturnFalse_WhenSaveChangesAsyncThrowsException()
		{
			// Arrange
			var existingPet = new Pet { Id = 1, Name = "Old Name" };
			var updatedPetVm = new PetEditViewModel { Id = 1, Name = "New Name" };

			_mockPetRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingPet);
			_mockPetRepository.Setup(r => r.SaveChangesAsync()).ThrowsAsync(new InvalidOperationException("Simulated DB error"));

			// Act
			var result = await _petService.UpdatePetAsync(updatedPetVm);

			// Assert
			Assert.False(result);
			_mockPetRepository.Verify(r => r.SaveChangesAsync(), Times.Once); // Verify save was attempted
		}

		[Fact]
		public async Task AddPetAsync_ShouldReturnFalse_WhenSaveChangesAsyncThrowsException()
		{
			// Arrange
			var newPetVm = new PetCreateViewModel
			{
				Name = "Buddy",
				Age = 3,
				Description = "Friendly dog",
				ImageUrl = "url",
				Gender = PetGender.Male,
				SpeciesId = 1,
				BreedId = 1,
				ShelterId = 1
			};

			_mockPetRepository.Setup(r => r.AddAsync(It.IsAny<Pet>())).Returns(Task.CompletedTask);
			_mockPetRepository.Setup(r => r.SaveChangesAsync()).ThrowsAsync(new InvalidOperationException("Simulated DB error"));

			// Act
			var result = await _petService.AddPetAsync(newPetVm, "adminId", true);

			// Assert
			Assert.False(result);
			_mockPetRepository.Verify(r => r.AddAsync(It.IsAny<Pet>()), Times.Once);
			_mockPetRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
		}
	}
}
