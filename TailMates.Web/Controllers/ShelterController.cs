﻿using Microsoft.AspNetCore.Mvc;
using TailMates.Services.Core.Interfaces;
using TailMates.Services.Core.Services;
using TailMates.Web.ViewModels.Shelter;

namespace TailMates.Web.Controllers
{
    public class ShelterController : BaseController
    {
        private readonly IShelterService shelterService;

		public ShelterController(IShelterService shelterService)
		{
            this.shelterService = shelterService;
        }

		public async Task<IActionResult> All()
		{
			
			var shelterViewModels = await shelterService.GetAllSheltersAsync();

			
			var viewModel = new ShelterListViewModel
			{
				Shelters = shelterViewModels
			};

			return View(viewModel);
		}

		public async Task<IActionResult> Details(int id)
		{
			var shelterDetails = await shelterService.GetShelterDetailsAsync(id);

			if (shelterDetails == null)
			{
				return NotFound();
			}

			return View(shelterDetails);
		}
	}
}
