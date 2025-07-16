using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TailMates.Data.Models;

namespace TailMates.Data.Repositories.Interfaces
{
	public interface IShelterRepository : IGenericRepository<Shelter>
	{
		Task<Shelter?> GetShelterByNameAsync(string name);
		Task<IEnumerable<Shelter>> GetAllSheltersWithPetsAsync(); 
		Task<Shelter?> GetShelterByIdWithPetsAsync(int id);
	}
}
