using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TailMates.Web.ViewModels;

namespace TailMates.Web.Controllers
{
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
			switch (statusCode)
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

		public IActionResult AdoptionProcess() 
		{
			ViewData["Title"] = "Our Adoption Process";
			return View();
		}
	}
}