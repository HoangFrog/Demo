using Microsoft.AspNetCore.Mvc;
using System.IO; //thao tac voi file, thu muc
using Newtonsoft.Json;//thao tac voi file json
using System.Data;//su dung DataTalbe, DataRow
using System.Data.SqlClient;//su dung SqlConnection, DataAdapter...
using X.PagedList;//su dung cac ham phan trang
using BC = BCrypt.Net;//doi tuong ma hoa csdl, gan doi tuong nay thanh BC
using project.Models;//nhan dien cac file trong thu muc Models
using System.Security.Cryptography;

namespace project.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MenusController : Controller
    {
        //doi tuong thao tac csdl
        public MyDbContext db = new MyDbContext();
        public IActionResult Index(int? page)
        {
            //khai bao so ban ghi tren mot trang
            int pageSize = 50;
            //tinh so trang
            int pageNumber = page ?? 1;//neu page = null thi PageNumber = 1; neu page != null thi PageNumber = page
            //List<ItemMenu> list_menu = db.Menus.OrderByDescending(item=>item.Id).ToList();
            //cach 2
            List<ItemMenu> list_menu = (from item in db.Menus orderby item.Arrange descending select item).ToList();
            return View(list_menu.ToPagedList(pageNumber, pageSize));
        }
        //update
        public IActionResult Update(int? id)
        {
            int _id = id ?? 0;
            //khai bao bien action de dua vao tham so action cua the form
            ViewBag.action = "/Admin/Menus/UpdatePost/" + _id;
            //lay mot ban ghi
            var record = db.Menus.Where(item => item.Id == _id).FirstOrDefault();
            return View("CreateUpdate", record);
        }
        //update post
        [HttpPost]
        public IActionResult UpdatePost(int? id, IFormCollection fc)
        {
            int _id = id ?? 0;
            //lay mot ban ghi
            var record = db.Menus.Where(item => item.Id == _id).FirstOrDefault();
            //---
            //ham .Trim() de cat khoang trang ben trai va ben phai cua chuoi
            string _Name = fc["Name"].ToString().Trim();//cac dung IFormCollection
            string _ControllerName = fc["ControllerName"].ToString().Trim();
            string _Link = fc["Link"].ToString().Trim();
            double _Arrange = Convert.ToDouble(fc["Arrange"]);
            if (record != null)
            {
                record.Name = _Name;
                record.ControllerName = _ControllerName; 
                record.Link = _Link;
                record.Arrange = _Arrange;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            //---
            return RedirectToAction("Index");
        }
        //create
        public IActionResult Create()
        {
            //khai bao bien action de dua vao tham so action cua the form
            ViewBag.action = "/Admin/Menus/CreatePost";
            return View("CreateUpdate");
        }
        //create post
        [HttpPost]
        public IActionResult CreatePost(IFormCollection fc)
        {
            string _Name = fc["Name"].ToString().Trim();
            string _ControllerName = fc["ControllerName"].ToString().Trim();
            string _Link = fc["Link"].ToString().Trim();
            //khoi tao ban ghi
            ItemMenu record = new ItemMenu();
            record.Name = _Name;
            record.ControllerName= _ControllerName;
            record.Link= _Link;
            //them ban ghi
            db.Menus.Add(record);
            //cap nhat thong tin 
            db.SaveChanges();
            //---
            //lay id vua insert
            int insertedId = record.Id;
            //lay ban ghi vua insert
            ItemMenu recordInserted = db.Menus.Where(item => item.Id == insertedId).FirstOrDefault();
            recordInserted.Arrange = insertedId;
            db.SaveChanges();
            //---
            return RedirectToAction("Index");
        }
        //delete
        public IActionResult Delete(int? id)
        {
            int _id = id ?? 0;
            //lay mot ban ghi
            ItemMenu record = db.Menus.Where(item => item.Id == _id).FirstOrDefault();
            //xoa ban ghi nay
            db.Menus.Remove(record);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
