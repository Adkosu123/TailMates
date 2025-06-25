using Microsoft.AspNetCore.Mvc;
using TailMates.Services.Core.Interfaces;
using TailMates.Web.ViewModels.Pet;

namespace TailMates.Web.Controllers
{
    public class PetController : Controller
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
    }
}
