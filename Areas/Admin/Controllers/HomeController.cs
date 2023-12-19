using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;
using project.Areas.Admin.Attributes;

namespace project.Areas.Admin.Controllers
{
	// phai dat tag sau o trong cac controller cuar Area = Admin
	[Area("Admin")]
	// kiem tra dang nhap
	[CheckLogin]
	
	
	public class HomeController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
