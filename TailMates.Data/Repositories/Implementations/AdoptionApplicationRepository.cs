using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TailMates.Data.Models;
using TailMates.Data.Models.Enums;
using TailMates.Data.Repositories.Interfaces;

namespace TailMates.Data.Repositories.Implementations
{
	public class AdoptionApplicationRepository : BaseRepository<AdoptionApplication>, IAdoptionApplicationRepository
	{
		public AdoptionApplicationRepository(TailMatesDbContext context) : base(context)
		{
		}

		public IQueryable<AdoptionApplication> GetAll()
		{
			return this._context.Set<AdoptionApplication>();
		}

		public async Task<bool> HasPendingApplicationForPetAndApplicantAsync(int petId, string applicantId)
		{
			return await _dbSet
						 .AnyAsync(a => a.PetId == petId &&
										a.ApplicantId == applicantId &&
										a.Status == ApplicationStatus.Pending);
		}

	}
}
