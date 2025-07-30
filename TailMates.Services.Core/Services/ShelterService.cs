using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TailMates.Data;
using TailMates.Data.Models;
using TailMates.Data.Repositories.Interfaces;
using TailMates.Services.Core.Interfaces;
using TailMates.Web.ViewModels.Pet;
using TailMates.Web.ViewModels.Shelter;

namespace TailMates.Services.Core.Services
{
	public class ShelterService : IShelterService
	{
		private readonly IShelterRepository shelterRepository;

		public ShelterService(IShelterRepository shelterRepository)
		{
			this.shelterRepository = shelterRepository;
		}

		public async Task<bool> AddShelterAsync(ShelterCreateViewModel model)
		{
			var shelter = new Shelter
			{
				Name = model.Name,
				Address = model.Address,
				Description = model.Description,
				PhoneNumber = model.PhoneNumber,
				Email = model.Email,
				ImageUrl = model.ImageUrl,
				IsDeleted = false 
			};

			await shelterRepository.AddAsync(shelter); 
			await shelterRepository.SaveChangesAsync(); 
			return true;
		}

		public async Task<IEnumerable<ShelterViewModel>> GetAllSheltersAsync()
		{
			var shelters = await shelterRepository
				.GetAllSheltersWithPetsAsync();

			shelters = shelters.Where(s => !s.IsDeleted);

        	var shelterViewModels = shelters.Select(s => new ShelterViewModel
			{
				Id = s.Id,
				Name = s.Name,
				Address = s.Address,
				PhoneNumber = s.PhoneNumber,
				ImageUrl = s.ImageUrl,
				Email = s.Email
			}).ToList();

			return shelterViewModels;
		}

		public async Task<ShelterDetailsViewModel?> GetShelterDetailsAsync(int id)
		{
			var shelter = await shelterRepository
				.GetShelterByIdWithPetsAsync(id);

			if (shelter == null || shelter.IsDeleted)
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
				Description = shelter.Description,
				ImageUrl = shelter.ImageUrl,
				Pets = shelter.Pets
								.Where(p => !p.IsDeleted && !p.IsAdopted)
								.Select(p => new PetViewModel
								{
									Id = p.Id,
									Name = p.Name,
									Age = p.Age,
									Description = p.Description,
									ImageUrl = p.ImageUrl,
									Gender = p.Gender.ToString(),
									SpeciesName = p.Species?.Name ?? "N/A",
									BreedName = p.Breed?.Name ?? "N/A",
									ShelterName = shelter.Name,
									ShelterId = p.ShelterId 
								})
								.ToList()
			};

			return shelterDetails;
		}
	}
}
