using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TailMates.Data;
using TailMates.Data.Repositories.Interfaces;
using TailMates.Services.Core.Interfaces;
using TailMates.Web.ViewModels.Pet;

namespace TailMates.Services.Core.Services
{
	public class PetService : IPetService
	{

		private readonly IPetRepository petRepository;

		public PetService(IPetRepository petRepository)
		{
			this.petRepository = petRepository;
		}
		public async Task<IEnumerable<PetViewModel>> GetAllPetsAsync()
		{
			var pets = await petRepository.GetAllPetsWithDetailsAsync();
	
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

		public async Task<PetDetailsViewModel?> GetPetDetailsAsync(int id)
		{
			var pet = await petRepository.GetPetByIdWithDetailsAsync(id);

			if (pet == null)
			{
				return null; 
			}

			var petDetails = new PetDetailsViewModel
			{
				Id = pet.Id,
				Name = pet.Name,
				Age = pet.Age,
				Description = pet.Description,
				ImageUrl = pet.ImageUrl,
				Gender = pet.Gender.ToString(),
				DateListed = pet.DateListed,
				IsAdopted = pet.IsAdopted,
				SpeciesName = pet.Species?.Name ?? "N/A", 
				BreedName = pet.Breed?.Name ?? "N/A",
				ShelterName = pet.Shelter?.Name ?? "N/A",
				ShelterAddress = pet.Shelter?.Address ?? "N/A",
				ShelterPhoneNumber = pet.Shelter?.PhoneNumber,
				ShelterEmail = pet.Shelter?.Email,
				ShelterId = pet.ShelterId,

			};

			return petDetails;
		}
	}
}
