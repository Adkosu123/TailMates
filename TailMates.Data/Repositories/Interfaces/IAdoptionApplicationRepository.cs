using TailMates.Data.Models;

namespace TailMates.Data.Repositories.Interfaces
{
	public interface IAdoptionApplicationRepository : IGenericRepository<AdoptionApplication>
	{
		Task<bool> HasPendingApplicationForPetAndApplicantAsync(int petId, string applicantId);

		IQueryable<AdoptionApplication> GetAll();
	}
}
