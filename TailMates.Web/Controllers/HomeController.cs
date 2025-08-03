namespace TailMates.Web.Controllers
{
	using System.Diagnostics;

	using ViewModels;

	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.Authorization;

	public class HomeController : BaseController
	{
		public HomeController(ILogger<HomeController> logger)
		{

		}

		[AllowAnonymous]
		public IActionResult Index()
		{
			return View();
		}


		[AllowAnonymous]
		public IActionResult Privacy()
		{
			return View();
		}

		public IActionResult AccessDenied()
		{
			return View("403Forbidden");
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error(int? statusCode = null)
		{
			if (statusCode.HasValue)
			{
				switch (statusCode.Value)
				{
					case 404:
						return View("404NotFound");
					default:
						return View();
				}
			}

			return View();
		}


		public IActionResult AdoptionProcess() 
		{
			ViewData["Title"] = "Our Adoption Process";
			return View();
		}
	}
}