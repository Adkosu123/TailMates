using TailMates.Web.ViewModels.AdoptionApplication;

namespace TailMates.Services.Core.Interfaces
{
	public interface IAdoptionApplicationService
	{
		Task<AdoptionApplicationCreateViewModel> GetAdoptionApplicationViewModelAsync(int petId);
		Task<bool> CreateAdoptionApplicationAsync(AdoptionApplicationCreateViewModel viewModel, string applicantId);
	}
}
