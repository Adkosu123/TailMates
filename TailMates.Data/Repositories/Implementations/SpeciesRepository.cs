using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TailMates.Data.Models;
using TailMates.Data.Repositories.Interfaces;

namespace TailMates.Data.Repositories.Implementations
{
	public class SpeciesRepository : BaseRepository<Species>, ISpeciesRepository
	{
		public SpeciesRepository(TailMatesDbContext context) : base(context)
		{
			
		}
	}
}
