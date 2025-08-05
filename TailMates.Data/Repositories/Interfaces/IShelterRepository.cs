using TailMates.Data.Models;

namespace TailMates.Data.Repositories.Interfaces
{
	public interface IShelterRepository : IGenericRepository<Shelter>
	{
		Task<Shelter?> GetShelterByNameAsync(string name);

		IQueryable<Shelter> GetAllSheltersWithPets();

		Task<Shelter?> GetShelterByIdWithPetsAsync(int id);
	}
}
