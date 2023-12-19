using Microsoft.AspNetCore.Mvc;
using project.Models;
using BC = BCrypt.Net.BCrypt; //thu vien ma hoa password

namespace project.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        //khoi tao doi tuong thao tac csdl
        public MyDbContext db = new MyDbContext();
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult LoginPost(IFormCollection fc)
        {
            string _Email = fc["Email"].ToString().Trim();
            string _Password = fc["Password"].ToString().Trim();
            // mã hoá password
            //_Password = BC.HashPassword(_Password);
            //return Content(_Password)
            // lấy một bản ghi tương ứng với email, password
            ItemUser record = db.Users.Where(item => item.Email == _Email).FirstOrDefault();
            if (record != null)
            {
                // BC là đối tượng của thư viện BCrypt
                if (BC.Verify(_Password, record.Password))
                {
                    //khởi tạo session Email
                    HttpContext.Session.SetString("admin_email", _Email);
                    //di chuyển đến url
                    return Redirect("/Admin/Home");
                }
            }
            return Redirect("/Admin/Account/Login");
        }
        //logout
        public IActionResult Logout()
        {
            //huy session
            HttpContext.Session.Remove("admin_email");
            return Redirect("/Admin/Account/Login");
        }
    }
}
