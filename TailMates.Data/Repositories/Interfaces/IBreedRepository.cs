using TailMates.Data.Models;

namespace TailMates.Data.Repositories.Interfaces
{
	public interface IBreedRepository : IGenericRepository<Breed>
	{
		Task<IEnumerable<Breed>> GetAllAsync();

		Task<IEnumerable<Breed>> GetBySpeciesIdAsync(int speciesId);
	}
}
