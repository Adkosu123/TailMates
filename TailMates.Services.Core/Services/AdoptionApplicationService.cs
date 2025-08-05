using TailMates.Data.Models;
using TailMates.Data.Models.Enums;
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

		public async Task<AdoptionApplicationCreateViewModel> GetAdoptionApplicationViewModelAsync(int petId)
		{
			var pet = await this.petRepository.GetPetByIdWithDetailsAsync(petId);

			if (pet == null)
			{
				return null;
			}

			var viewModel = new AdoptionApplicationCreateViewModel
			{
				PetId = pet.Id,
				PetName = pet.Name,
				PetSpecies = pet.Species.Name,
				PetBreed = pet.Breed.Name,
				PetAge = pet.Age,
				PetImageUrl = pet.ImageUrl,
				PetDescription = pet.Description
			};

			return viewModel;
		}

		public async Task<bool> CreateAdoptionApplicationAsync(AdoptionApplicationCreateViewModel viewModel, string applicantId)
		{
			var pet = await petRepository.GetAvailablePetByIdAsync(viewModel.PetId);

			if (pet == null)
			{
				return false;
			}

			var existingPendingApplication = await adoptionApplicationRepository
												.HasPendingApplicationForPetAndApplicantAsync(viewModel.PetId, applicantId);

			if (existingPendingApplication)
			{
				return false;
			}

			var application = new AdoptionApplication
			{
				PetId = viewModel.PetId,
				ApplicantId = applicantId,
				ApplicationDate = DateTime.UtcNow,
				Status = ApplicationStatus.Pending,
				ApplicantNotes = viewModel.ApplicantNotes
			};

			await adoptionApplicationRepository.AddAsync(application);
			await adoptionApplicationRepository.SaveChangesAsync();

			return true;
		}
	}
}
