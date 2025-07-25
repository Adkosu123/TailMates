﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TailMates.Data.Models;

namespace TailMates.Data.Repositories.Interfaces
{
	public interface IBreedRepository : IGenericRepository<Breed>
	{
		Task<IEnumerable<Breed>> GetAllAsync();

		Task<IEnumerable<Breed>> GetBySpeciesIdAsync(int speciesId);
	}
}
