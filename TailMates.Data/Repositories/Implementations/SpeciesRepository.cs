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
