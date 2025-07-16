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
		Task<IEnumerable<Pet>> GetAllPetsWithDetailsAsync();
		Task<Pet?> GetPetByIdWithDetailsAsync(int id);
		Task<IEnumerable<Pet>> GetAvailablePetsAsync();

		Task<Pet?> GetAvailablePetByIdAsync(int petId);
	}
}
