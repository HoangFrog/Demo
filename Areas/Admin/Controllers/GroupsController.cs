using Microsoft.AspNetCore.Mvc;
using System.IO; //thao tac voi file, thu muc
using Newtonsoft.Json;//thao tac voi file json
using System.Data;//su dung DataTalbe, DataRow
using System.Data.SqlClient;//su dung SqlConnection, DataAdapter...
using X.PagedList;//su dung cac ham phan trang
using BC = BCrypt.Net;//doi tuong ma hoa csdl, gan doi tuong nay thanh BC
using project.Models;//nhan dien cac file trong thu muc Models
using System.Security.Cryptography;
using project.Areas.Admin.Attributes;


namespace project.Areas.Admin.Controllers
{
    [Area("Admin")]
    [CheckLogin]
    public class GroupsController : Controller
    {
        //doi tuong thao tac csdl
        public MyDbContext db = new MyDbContext();
        public IActionResult Index(int? page)
        {
            //khai bao so ban ghi tren mot trang
            int pageSize = 50;
            //tinh so trang
            int pageNumber = page ?? 1;//neu page = null thi PageNumber = 1; neu page != null thi PageNumber = page
            //List<ItemPermissionGroup> list_permission_group = db.Groups.OrderByDescending(item=>item.Id).ToList();
            //cach 2
            List<ItemGroup> list_permission_group = (from item in db.Groups orderby item.Arrange descending select item).ToList();
            return View(list_permission_group.ToPagedList(pageNumber, pageSize));
        }
        //update
        public IActionResult Update(int? id)
        {
            int _id = id ?? 0;
            //khai bao bien action de dua vao tham so action cua the form
            ViewBag.action = "/Admin/Groups/UpdatePost/" + _id;
            //lay mot ban ghi
            var record = db.Groups.Where(item => item.Id == _id).FirstOrDefault();
            return View("CreateUpdate", record);
        }
        //update post
        [HttpPost]
        public IActionResult UpdatePost(int? id, IFormCollection fc)
        {
            int _id = id ?? 0;
            //lay mot ban ghi
            var record = db.Groups.Where(item => item.Id == _id).FirstOrDefault();
            //---
            //ham .Trim() de cat khoang trang ben trai va ben phai cua chuoi
            string _Name = fc["Name"].ToString().Trim();//cac dung IFormCollection
            double _Arrange = Convert.ToDouble(fc["Arrange"]);
            if (record != null)
            {
                record.Name = _Name;
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
            ViewBag.action = "/Admin/Groups/CreatePost";
            return View("CreateUpdate");
        }
        //create post
        [HttpPost]
        public IActionResult CreatePost(IFormCollection fc)
        {
            string _Name = fc["Name"].ToString().Trim();
            //khoi tao ban ghi
            ItemGroup record = new ItemGroup();
            record.Name = _Name;
            //them ban ghi
            db.Groups.Add(record);
            //cap nhat thong tin 
            db.SaveChanges();
            //---
            //lay id vua insert
            int insertedId = record.Id;
            //lay ban ghi vua insert
            ItemGroup recordInserted = db.Groups.Where(item => item.Id == insertedId).FirstOrDefault();
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
            ItemGroup record = db.Groups.Where(item => item.Id == _id).FirstOrDefault();
            //xoa ban ghi nay
            db.Groups.Remove(record);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult SetMenus(int id)
        {
            ViewBag.GroupId = id;
            List<ItemMenu> list_record = db.Menus.OrderByDescending(item => item.Id).ToList();
            return View(list_record);
        }
        public IActionResult SetMenusPost(int? id)
        {
            int _GroupId = id ?? 0;
            //---
            //xoa toan bo phan quyen cu trong table MenusGroups
            var list_old_permission = db.MenusGroups.Where(item => item.GroupId == _GroupId).ToList();
            foreach (var item_old_permission in list_old_permission)
            {                
                //---
                db.MenusGroups.Remove(item_old_permission);
                db.SaveChanges();
            }
            //---
            var _ListIdMenus = Request.Form["Menu"];
            foreach(string _MenuId in _ListIdMenus)
            {                
                //---
                ItemMenuGroup record = new ItemMenuGroup();
                record.MenuId = Convert.ToInt32(_MenuId);
                record.GroupId = _GroupId;
                db.MenusGroups.Add(record);
                db.SaveChanges() ;
                //---
            }
            return Redirect("/Admin/Groups");
        }
        public IActionResult SetUsers(int id)
        {
            ViewBag.GroupId = id;
            List<ItemUser> list_record = db.Users.OrderByDescending(item => item.Id).ToList();
            return View(list_record);
        }
        public IActionResult SetUsersPost(int? id)
        {
            int _GroupId = id ?? 0;
            var _ListIdUsers = Request.Form["User"];
            //---
            //---
            // chỗ này thầy đã xóa nhưng mà chưa chắc , có gì xem lại sau
            //xoa toan bo phan quyen cua user thuoc group nay de phan lai quyen moi
            var list_old_permission_user = db.UsersGroups.Where(item => item.GroupId == _GroupId).ToList();
            foreach (var itemUserId in list_old_permission_user)
            {                
                db.UsersGroups.Remove(itemUserId);
                db.SaveChanges();
            }
            //---
            //phan lai quyen moi cho user
            foreach (string _UserId in _ListIdUsers)
            {
                //---
                //xoa user nay de phan lai quyen
                var current_user = db.UsersGroups.Where(item => item.UserId == Convert.ToInt32(_UserId)).ToList();
                foreach(var item_current_user in current_user)
                {
                    db.UsersGroups.Remove(item_current_user);
                    db.SaveChanges();
                }
                //---
                //them moi lai user
                ItemUserGroup record = new ItemUserGroup();
                record.UserId = Convert.ToInt32(_UserId);
                record.GroupId = _GroupId;
                db.UsersGroups.Add(record);
                db.SaveChanges();
                //---
            }
            return Redirect("/Admin/Groups");
        }
    }
}
