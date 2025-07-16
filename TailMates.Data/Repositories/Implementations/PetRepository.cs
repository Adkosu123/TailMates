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
	}
}
