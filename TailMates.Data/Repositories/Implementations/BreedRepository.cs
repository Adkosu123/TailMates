using Microsoft.EntityFrameworkCore;
using TailMates.Data.Models;
using TailMates.Data.Repositories.Interfaces;

namespace TailMates.Data.Repositories.Implementations
{
	public class BreedRepository : BaseRepository<Breed>, IBreedRepository
	{
		public BreedRepository(TailMatesDbContext context) : base(context)
		{

		}

		public async Task<IEnumerable<Breed>> GetAllAsync()
		{
			return await _context.Breeds.ToListAsync();
		}

		public async Task<IEnumerable<Breed>> GetBySpeciesIdAsync(int speciesId) 
		{
			return await _context.Breeds.Where(b => b.SpeciesId == speciesId).ToListAsync();
		}

	}
}
