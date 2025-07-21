using Microsoft.EntityFrameworkCore;
using TailMates.Data.Models;
using TailMates.Data.Repositories.Interfaces;

namespace TailMates.Data.Repositories.Implementations
{
	public class ShelterRepository : BaseRepository<Shelter>, IShelterRepository
	{
		public ShelterRepository(TailMatesDbContext context) : base(context)
		{
		}

		public async Task<Shelter?> GetShelterByNameAsync(string name)
		{
			return await _dbSet.FirstOrDefaultAsync(s => s.Name == name);
		}
		public async Task<IEnumerable<Shelter>> GetAllSheltersWithPetsAsync()
		{
			return await _dbSet
				.Include(s => s.Pets)
				.ToListAsync();
		}

		public async Task<Shelter?> GetShelterByIdWithPetsAsync(int id)
		{
			return await _dbSet
				.Include(s => s.Pets)
				.ThenInclude(p => p.Species)
				.Include(s => s.Pets)
				.ThenInclude(p => p.Breed)
				.FirstOrDefaultAsync(s => s.Id == id);
		}

	}
}
