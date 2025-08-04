using Microsoft.AspNetCore.Mvc;

namespace TailMates.Web.Areas.Admin.Controllers
{
	public class HomeController : BaseAdminController
	{
		public IActionResult Index()
		{
			return this.View();
		}
	}
}
