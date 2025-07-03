using Microsoft.AspNetCore.Mvc;
using TailMates.Services.Core.Interfaces;
using TailMates.Services.Core.Services;
using TailMates.Web.ViewModels.Pet;

namespace TailMates.Web.Controllers
{
    public class PetController : BaseController
    {
		private readonly IPetService petService;

		public PetController(IPetService petService)
		{
			this.petService = petService;
		}

		public async Task<IActionResult> All()
        {
			var petViewModels = await petService.GetAllPetsAsync();
			var viewModel = new PetListViewModel
			{
				Pets = petViewModels
			};

			return View(viewModel);
		}

		public async Task<IActionResult> Details(int id) 
		{
			var petDetails = await petService.GetPetDetailsAsync(id);

			if (petDetails == null)
			{
				return NotFound();
			}
			return View(petDetails);
		}
    }
}
