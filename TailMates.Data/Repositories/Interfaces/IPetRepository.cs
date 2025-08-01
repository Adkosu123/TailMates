using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TailMates.Data.Models;

namespace TailMates.Data.Repositories.Interfaces
{
	public interface IPetRepository : IGenericRepository<Pet>
	{
		IQueryable<Pet> GetAllPetsWithDetails();

		IQueryable<Pet> AllAsNoTracking();

		Task<PaginatedList<Pet>> GetFilteredPetsAsync(string? searchTerm, int? speciesId, int? breedId, string? gender, int? minAge, int? maxAge, int? shelterId, int pageIndex, int pageSize);

		Task<IEnumerable<Species>> GetAllSpeciesLookupAsync();
		Task<IEnumerable<Shelter>> GetAllSheltersLookupAsync();
		Task<IEnumerable<Breed>> GetBreedsForSpeciesLookupAsync(int speciesId);

		Task<IEnumerable<Pet>> GetAllPetsWithDetailsAsync();
		Task<Pet?> GetPetByIdWithDetailsAsync(int id);
		Task<IEnumerable<Pet>> GetAvailablePetsAsync();

		IQueryable<Pet> GetPetsByShelterId(int shelterId);

		Task<Pet?> GetAvailablePetByIdAsync(int petId);
	}
}
