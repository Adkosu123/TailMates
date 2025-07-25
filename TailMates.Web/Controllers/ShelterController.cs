using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using TailMates.Services.Core.Interfaces;
using TailMates.Services.Core.Services;
using TailMates.Web.ViewModels.Shelter;

namespace TailMates.Web.Controllers
{
    public class ShelterController : BaseController
    {
        private readonly IShelterService shelterService;
		private readonly ILogger<ShelterController> logger;
		public ShelterController(IShelterService shelterService,
			ILogger<ShelterController> logger)
		{
            this.shelterService = shelterService;
			this.logger = logger;
        }

		public async Task<IActionResult> All()
		{

			try
			{
				var shelterViewModels = await shelterService.GetAllSheltersAsync();


				var viewModel = new ShelterListViewModel
				{
					Shelters = shelterViewModels
				};

				return View(viewModel);
			}
			catch (Exception e)
			{
				this.logger.LogError(e.Message);
				return this.RedirectToAction("Index", "Home");
			}
		}

		public async Task<IActionResult> Details(int id)
		{
			try
			{
				var shelterDetails = await shelterService.GetShelterDetailsAsync(id);

				if (shelterDetails == null)
				{
					return NotFound();
				}

				return View(shelterDetails);
			}
			catch (Exception e)
			{
				this.logger.LogError(e.Message);
				return this.RedirectToAction("All", "Shelter");
			}
		}
	}
}
