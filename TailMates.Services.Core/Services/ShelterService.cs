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
		private readonly IPetRepository petRepository;

		public ShelterService(IShelterRepository shelterRepository,
			IPetRepository petRepository)
		{
			this.shelterRepository = shelterRepository;
			this.petRepository = petRepository;
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

		public async Task<PaginatedList<ShelterViewModel>> GetAllSheltersAsync(int pageIndex, int pageSize)
		{
			var allSheltersQuery = this.shelterRepository.GetAllSheltersWithPets();

			var paginatedShelters = await PaginatedList<Shelter>.CreateAsync(allSheltersQuery, pageIndex, pageSize);

			var viewModelList = paginatedShelters
				.Select(s => new ShelterViewModel
				{
					Id = s.Id,
					Name = s.Name,
					Address = s.Address,
					ImageUrl = s.ImageUrl,
					PhoneNumber = s.PhoneNumber,
					Email = s.Email,
				})
				.ToList();

			return new PaginatedList<ShelterViewModel>(
				viewModelList,
				paginatedShelters.TotalCount,
				paginatedShelters.PageIndex,
				paginatedShelters.PageSize
			);
		}

		public async Task<ShelterDetailsViewModel> GetShelterDetailsWithPaginatedPetsAsync(int shelterId, int pageIndex, int pageSize)
		{
			var shelter = await this.shelterRepository.GetShelterByIdWithPetsAsync(shelterId);

			if (shelter == null)
			{
				return null;
			}

			var petsQuery = this.petRepository.GetPetsByShelterId(shelterId);
			var paginatedPets = await PaginatedList<Pet>.CreateAsync(petsQuery, pageIndex, pageSize);

			var petViewModels = paginatedPets.Select(p => new PetViewModel
			{
				Id = p.Id,
				Name = p.Name,
				Age = p.Age,
				Gender = p.Gender.ToString(),
				SpeciesName = p.Species.Name,
				BreedName = p.Breed.Name,
				ImageUrl = p.ImageUrl,
			}).ToList();

			var paginatedPetViewModels = new PaginatedList<PetViewModel>(
				petViewModels,
				paginatedPets.TotalCount,
				paginatedPets.PageIndex,
				paginatedPets.PageSize
			);

			var shelterDetailsViewModel = new ShelterDetailsViewModel
			{
				Id = shelter.Id,
				Name = shelter.Name,
				Address = shelter.Address,
				PhoneNumber = shelter.PhoneNumber,
				Email = shelter.Email,
				Description = shelter.Description,
				ImageUrl = shelter.ImageUrl,
				Pets = paginatedPetViewModels
			};

			return shelterDetailsViewModel;
		}

		public async Task<ShelterEditViewModel?> GetShelterForEditAsync(int id)
		{
			var shelter = await this.shelterRepository.GetByIdAsync(id);
			if (shelter == null)
			{
				return null;
			}

			return new ShelterEditViewModel
			{
				Id = shelter.Id,
				Name = shelter.Name,
				Address = shelter.Address,
				Description = shelter.Description,
				PhoneNumber = shelter.PhoneNumber,
				Email = shelter.Email,
				ImageUrl = shelter.ImageUrl
			};
		}

		public async Task<bool> UpdateShelterAsync(ShelterEditViewModel model)
		{
			var shelter = await this.shelterRepository.GetByIdAsync(model.Id);
			if (shelter == null)
			{
				return false;
			}

			shelter.Name = model.Name;
			shelter.Address = model.Address;
			shelter.Description = model.Description;
			shelter.PhoneNumber = model.PhoneNumber;
			shelter.Email = model.Email;
			shelter.ImageUrl = model.ImageUrl;

			this.shelterRepository.Update(shelter);
			await this.shelterRepository.SaveChangesAsync();
			return true;
		}
	}
}
