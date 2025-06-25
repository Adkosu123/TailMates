using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TailMates.Data;
using TailMates.Services.Core.Interfaces;
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
	}
}
