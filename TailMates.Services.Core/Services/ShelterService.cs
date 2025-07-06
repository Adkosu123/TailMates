using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TailMates.Data;
using TailMates.Services.Core.Interfaces;
using TailMates.Web.ViewModels.Pet;
using TailMates.Web.ViewModels.Shelter;

namespace TailMates.Services.Core.Services
{
	public class ShelterService : IShelterService
	{
		private readonly TailMatesDbContext dbContext;

		public ShelterService(TailMatesDbContext dbContext)
		{
			this.dbContext = dbContext;
		}

		public async Task<IEnumerable<ShelterViewModel>> GetAllSheltersAsync()
		{
			var shelters = await dbContext.Shelters
			   .ToListAsync();

			
			var shelterViewModels = shelters.Select(s => new ShelterViewModel
			{
				Id = s.Id,
				Name = s.Name,
				Address = s.Address,
				PhoneNumber = s.PhoneNumber,
				Email = s.Email
			}).ToList();

			return shelterViewModels;
		}

		public async Task<ShelterDetailsViewModel?> GetShelterDetailsAsync(int id)
		{
			var shelter = await dbContext.Shelters
			   .Include(s => s.Pets) 
				   .ThenInclude(p => p.Species) 
			   .Include(s => s.Pets)
				   .ThenInclude(p => p.Breed) 
			   .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);

			if (shelter == null)
			{
				return null;
			}

			var shelterDetails = new ShelterDetailsViewModel
			{
				Id = shelter.Id,
				Name = shelter.Name,
				Address = shelter.Address,
				PhoneNumber = shelter.PhoneNumber,
				Email = shelter.Email,
				Pets = shelter.Pets
								.Where(p => !p.IsDeleted && !p.IsAdopted) // Only show available, non-deleted pets
								.Select(p => new PetViewModel // Reuse AllPetsViewModel structure
								{
									Id = p.Id,
									Name = p.Name,
									Age = p.Age,
									Description = p.Description,
									ImageUrl = p.ImageUrl,
									Gender = p.Gender.ToString(),
									SpeciesName = p.Species?.Name ?? "N/A",
									BreedName = p.Breed?.Name ?? "N/A",
									ShelterName = shelter.Name // This will be the current shelter's name
								})
								.ToList()
			};

			return shelterDetails;
		}
	}
}
