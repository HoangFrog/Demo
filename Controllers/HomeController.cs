using Microsoft.AspNetCore.Mvc;

namespace project.Controllers
{
	public class HomeController : Controller
	{
		public IActionResult Index()
		{
			return View();
			// tra ve admin
			//return Redirect("/Admin");
		}
	}
}
