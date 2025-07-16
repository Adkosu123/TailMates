using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TailMates.Data.Models;

namespace TailMates.Data.Repositories.Interfaces
{
	public interface IAdoptionApplicationRepository : IGenericRepository<AdoptionApplication>
	{
		Task<bool> HasPendingApplicationForPetAndApplicantAsync(int petId, string applicantId);
	}
}
