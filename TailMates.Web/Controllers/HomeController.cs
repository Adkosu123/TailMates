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
			return this.RedirectToAction("Error", "Home", new { statusCode = 403 });
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error(int? statusCode)
		{
			if (statusCode.HasValue)
			{
				switch (statusCode.Value)
				{
					case 401:
					case 403:
						return this.View("403Forbidden");
					case 404:
						return this.View("404NotFound");
					default:
						ErrorViewModel model = new ErrorViewModel
						{
							RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
						};
						return this.View(model);
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