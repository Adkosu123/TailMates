using Microsoft.EntityFrameworkCore;
using TailMates.Data.Models;
using TailMates.Data.Models.Enums;
using TailMates.Data.Repositories.Interfaces;

namespace TailMates.Data.Repositories.Implementations
{
	public class PetRepository : BaseRepository<Pet>, IPetRepository
	{
		public PetRepository(TailMatesDbContext context) : base(context)
		{
		}

		public async Task<IEnumerable<Pet>> GetAllPetsWithDetailsAsync()
		{
			return await _dbSet
				.Include(p => p.Breed)
				.Include(p => p.Species)
				.Include(p => p.Shelter)
				.ToListAsync();
		}

		public async Task<Pet?> GetPetByIdWithDetailsAsync(int id)
		{
			return await _dbSet
				.Include(p => p.Breed)
				.Include(p => p.Species)
				.Include(p => p.Shelter)
				.FirstOrDefaultAsync(p => p.Id == id);
		}

		public async Task<IEnumerable<Pet>> GetAvailablePetsAsync()
		{
			return await _dbSet
				.Where(p => p.PetStatus == PetStatus.Available) 
				.Include(p => p.Breed)
				.Include(p => p.Species)
				.Include(p => p.Shelter)
				.ToListAsync();
		}

		public async Task<Pet?> GetAvailablePetByIdAsync(int petId)
		{

			return await _dbSet
						 .Where(p => !p.IsDeleted && p.PetStatus != PetStatus.Adopted) // Check for deleted and adopted status
						 .FirstOrDefaultAsync(p => p.Id == petId);
		}

		public IQueryable<Pet> GetAllPetsWithDetails()
		{
			return _dbSet 
				.Include(p => p.Breed)
				.Include(p => p.Species)
				.Include(p => p.Shelter)
				.Where(p => !p.IsDeleted);
		}

		public async Task<IEnumerable<Species>> GetAllSpeciesLookupAsync()
		{
			return await _context.Species.OrderBy(s => s.Name).ToListAsync();
		}

		public async Task<IEnumerable<Shelter>> GetAllSheltersLookupAsync()
		{
			return await _context.Shelters.OrderBy(s => s.Name).ToListAsync();
		}

		public async Task<IEnumerable<Breed>> GetBreedsForSpeciesLookupAsync(int speciesId)
		{
			return await _context.Breeds
								 .Where(b => b.SpeciesId == speciesId)
								 .OrderBy(b => b.Name)
								 .ToListAsync();
		}

		public IQueryable<Pet> AllAsNoTracking()
		{
			return _dbSet.AsNoTracking();
		}

		public async Task<PaginatedList<Pet>> GetFilteredPetsAsync(string? searchTerm, int? speciesId, int? breedId, string? gender, int? minAge, int? maxAge, int? shelterId, int pageIndex, int pageSize)
		{
			var petsQuery = _context.Pets
				.Where(p => !p.IsDeleted && !p.IsAdopted)
				.Include(p => p.Species)
				.Include(p => p.Breed)
				.Include(p => p.Shelter)
				.AsQueryable();

			if (!string.IsNullOrWhiteSpace(searchTerm))
			{
				petsQuery = petsQuery.Where(p => p.Name.Contains(searchTerm) ||
												p.Description.Contains(searchTerm) ||
												(p.Species != null && p.Species.Name.Contains(searchTerm)) ||
												(p.Breed != null && p.Breed.Name.Contains(searchTerm)) ||
												(p.Shelter != null && p.Shelter.Name.Contains(searchTerm)));
			}

			if (speciesId.HasValue && speciesId.Value > 0)
			{
				petsQuery = petsQuery.Where(p => p.SpeciesId == speciesId.Value);
			}

			if (breedId.HasValue && breedId.Value > 0)
			{
				petsQuery = petsQuery.Where(p => p.BreedId == breedId.Value);
			}

			if (!string.IsNullOrWhiteSpace(gender))
			{
				if (Enum.TryParse<PetGender>(gender, true, out var genderEnum))
				{
					petsQuery = petsQuery.Where(p => p.Gender == genderEnum);
				}
			}

			if (minAge.HasValue)
			{
				petsQuery = petsQuery.Where(p => p.Age >= minAge.Value);
			}

			if (maxAge.HasValue)
			{
				petsQuery = petsQuery.Where(p => p.Age <= maxAge.Value);
			}

			if (shelterId.HasValue && shelterId.Value > 0)
			{
				petsQuery = petsQuery.Where(p => p.ShelterId == shelterId.Value);
			}

			return await PaginatedList<Pet>.CreateAsync(petsQuery.OrderBy(p => p.Name), pageIndex, pageSize);
		}

		public IQueryable<Pet> GetPetsByShelterId(int shelterId)
		{
			return this._context.Pets.Where(p => p.ShelterId == shelterId);
		}
	}
}
