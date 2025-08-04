using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TailMates.Data.Models;
using TailMates.Data.Repositories.Interfaces;

namespace TailMates.Data.Repositories.Implementations
{
	public class MyAdoptionApplicationsRepository : BaseRepository<AdoptionApplication>, IMyAdoptionApplicationsRepository
	{
		public MyAdoptionApplicationsRepository(TailMatesDbContext context) : base(context)
		{
		}

		public IQueryable<AdoptionApplication> GetAllByApplicantId(string applicantId)
		{
			return this._dbSet.Where(a => a.ApplicantId == applicantId).AsQueryable();
		}
	}
}
