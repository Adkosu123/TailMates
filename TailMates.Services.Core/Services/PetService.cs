using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TailMates.Data;
using TailMates.Services.Core.Interfaces;
using TailMates.Web.ViewModels.Pet;

namespace TailMates.Services.Core.Services
{
	public class PetService : IPetService
	{

		private readonly TailMatesDbContext dbContext;

		public PetService(TailMatesDbContext dbContext)
		{
			this.dbContext = dbContext;
		}
		public async Task<IEnumerable<PetViewModel>> GetAllPetsAsync()
		{
			var pets = await dbContext.Pets
				.Include(p => p.Species) 
				.Include(p => p.Breed)   
				.Include(p => p.Shelter) 
				.ToListAsync();

			
			var petViewModels = pets.Select(p => new PetViewModel
			{
				Id = p.Id,
				Name = p.Name,
				Age = p.Age,
				Description = p.Description,
				ImageUrl = p.ImageUrl,
				Gender = p.Gender.ToString(), 
				SpeciesName = p.Species?.Name ?? "N/A", 
				BreedName = p.Breed?.Name ?? "N/A",
				ShelterName = p.Shelter?.Name ?? "N/A"
			}).ToList();

			return petViewModels;
		}
	}
}
