using Microsoft.AspNetCore.Authorization;
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

		[HttpGet]
		[Authorize(Roles = "Admin")] 
		public IActionResult Add()
		{
			return View();
		}

		[HttpPost]
		[Authorize(Roles = "Admin")] 
		public async Task<IActionResult> Add(ShelterCreateViewModel model)
		{

			try
			{
				if (!ModelState.IsValid)
				{
					TempData["Message"] = "Validation failed. Please check your inputs.";
					TempData["MessageType"] = "error";
					return View(model);
				}

				bool success = await shelterService.AddShelterAsync(model);

				if (success)
				{
					TempData["Message"] = $"Shelter '{model.Name}' added successfully!";
					TempData["MessageType"] = "success";
					return RedirectToAction("All", "Shelter");
				}
				else
				{
					TempData["Message"] = $"Failed to add shelter '{model.Name}'. Please try again.";
					TempData["MessageType"] = "error";
					return View(model);
				}
			}
			catch (Exception e)
			{
				this.logger.LogError(e.Message);
				return this.RedirectToAction("All", "Shelter");
			}
			
		}
	}
}
