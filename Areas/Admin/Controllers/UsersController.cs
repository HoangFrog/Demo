using Microsoft.AspNetCore.Mvc;
using System.IO; //thao tac voi file, thu muc
using Newtonsoft.Json;//thao tac voi file json
using System.Data;//su dung DataTalbe, DataRow
using System.Data.SqlClient;//su dung SqlConnection, DataAdapter...
using X.PagedList;//su dung cac ham phan trang
using BC = BCrypt.Net;//doi tuong ma hoa csdl, gan doi tuong nay thanh BC
using project.Models;//nhan dien cac file trong thu muc Models
using System.Security.Cryptography;
using project.Areas.Admin.Attributes;//de nhin thay class CheckLogin.cs


// su dung Linq
namespace projectnhom14.Areas.Admin.Controllers
{
    [Area("Admin")]
    [CheckLogin]
    public class UsersController : Controller
    {
        //doi tuong thao tac csdl
        public MyDbContext db = new MyDbContext();
        public IActionResult Index(int? page)
        {
            //khai bao so ban ghi tren mot trang
            int pageSize = 4;
            //tinh so trang
            int pageNumber = page ?? 1;//neu page = null thi PageNumber = 1; neu page != null thi PageNumber = page
            //List<ItemUser> list_users = db.Users.OrderByDescending(item=>item.Id).ToList();
            //cach 2
            List<ItemUser> list_users = (from item in db.Users orderby item.Id descending select item).ToList();
            return View(list_users.ToPagedList(pageNumber, pageSize));
        }
        //update
        public IActionResult Update(int? id)
        {
            int _id = id ?? 0;
            //khai bao bien action de dua vao tham so action cua the form
            ViewBag.action = "/Admin/Users/UpdatePost/" + _id;
            //lay mot ban ghi
            var record = db.Users.Where(item => item.Id == _id).FirstOrDefault();
            return View("CreateUpdate", record);
        }
        //update post
        [HttpPost]
        public IActionResult UpdatePost(int? id, IFormCollection fc)
        {
            int _id = id ?? 0;
            //lay mot ban ghi
            var record = db.Users.Where(item => item.Id == _id).FirstOrDefault();
            //---
            //ham .Trim() de cat khoang trang ben trai va ben phai cua chuoi
            string _Name = fc["Name"].ToString().Trim();//cac dung IFormCollection
            string _Email = Request.Form["Email"].ToString().Trim();//cach dung Request.Form
            string _Password = Request.Form["Password"].ToString().Trim();
            if (record != null)
            {
                //---
                //kiem tra xem Email da ton tai chua, neu chua ton tai thi moi cho update
                var check = db.Users.Where(item => item.Email == _Email && item.Id != _id).FirstOrDefault();
                if (check == null)
                {
                    record.Email = _Email;
                    record.Name = _Name;
                    //neu password khong rong thi update password
                    if (!String.IsNullOrEmpty(_Password))
                    {
                        //ma hoa password
                        _Password = BC.BCrypt.HashPassword(_Password);
                        record.Password = _Password;
                    }
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                    return Redirect("/Admin/Users/Update/" + _id + "?notify=email-exists");
                //---                
            }
            //---
            return RedirectToAction("Index");
        }
        //create
        public IActionResult Create()
        {
            //khai bao bien action de dua vao tham so action cua the form
            ViewBag.action = "/Admin/Users/CreatePost";
            return View("CreateUpdate");
        }
        //create post
        [HttpPost]
        public IActionResult CreatePost(IFormCollection fc)
        {
            string _Name = fc["Name"].ToString().Trim();
            string _Email = fc["Email"].ToString().Trim();
            string _Password = fc["Password"].ToString().Trim();
            //ma hoa password
            _Password = BC.BCrypt.HashPassword(_Password);
            //kiem tra xem email da ton tai chua, neu chua ton tai thi moi cho insert ban ghi
            //.Count() tra ve ket qua co bao nhieu ban ghi (so)
            var check = db.Users.Where(item => item.Email == _Email).Count();
            //return Content(check.ToString());
            if (check == 0)
            {
                //khoi tao ban ghi
                ItemUser record = new ItemUser();
                record.Name = _Name;
                record.Email = _Email;
                record.Password = _Password;
                //them ban ghi
                db.Users.Add(record);
                //cap nhat thong tin 
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
                return Redirect("/Admin/Users/Create?notify=email-exists");
        }
        //delete
        public IActionResult Delete(int? id)
        {
            int _id = id ?? 0;
            // lay mot ban ghi
            ItemUser record = db.Users.Where(item => item.Id == id).FirstOrDefault();
            // xoa an ghi nay
            db.Users.Remove(record);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
