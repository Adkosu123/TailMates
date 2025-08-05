using TailMates.Data.Models;

namespace TailMates.Data.Repositories.Interfaces
{
	public interface IMyAdoptionApplicationsRepository :IGenericRepository<AdoptionApplication>
	{
		IQueryable<AdoptionApplication> GetAllByApplicantId(string applicantId);
	}
}
