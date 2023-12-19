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


namespace project.Areas.Admin.Controllers
{
    [Area("Admin")]
    [CheckLogin]
    public class ProductsController : Controller
    {
        //khoi tao doi tuong thao tac csdl
        public MyDbContext db = new MyDbContext();
        public IActionResult Index(int? page)
        {
            //khai bao so ban ghi tren mot trang
            int pageSize = 7;
            //tinh so trang
            int pageNumber = page ?? 1;
            List<ItemProduct> list_record = (from item in db.Products orderby item.Id descending select item).ToList();
            return View(list_record.ToPagedList(pageNumber, pageSize));
        }
        //update
        public IActionResult Update(int? id)
        {
            int _id = id ?? 0;
            //khai bao bien action de dua vao tham so action cua the form
            ViewBag.action = "/Admin/Products/UpdatePost/" + _id;
            //lay mot ban ghi
            var record = db.Products.Where(item => item.Id == _id).FirstOrDefault();
            return View("CreateUpdate", record);
        }
        //update post
        [HttpPost]
        public IActionResult UpdatePost(int? id, IFormCollection fc)
        {
            int _id = id ?? 0;
            //lay mot ban ghi
            var record = db.Products.Where(item => item.Id == _id).FirstOrDefault();
            //---
            //ham .Trim() de cat khoang trang ben trai va ben phai cua chuoi
            string _Name = fc["Name"].ToString().Trim();//cac dung IFormCollection
            string _Description = Request.Form["Description"].ToString().Trim();//cach dung Request.Form
            string _Content = Request.Form["Content"].ToString().Trim();
            int _Hot = !String.IsNullOrEmpty(Request.Form["Hot"]) ? 1 : 0;
            double _Price = Convert.ToDouble(Request.Form["Price"]);
            double _Discount = Convert.ToDouble(Request.Form["Discount"]);
            var SelectedCategories = Request.Form["Categories"]; // do the selcet la mutiple nen Categories se la 1 array bao gom danh sach cac iid
            var SelectedTags = Request.Form["Tags"];


            if (record != null)
            {
                //---
                record.Name = _Name;
                record.Description = _Description;
                record.Content = _Content;
                record.Hot = _Hot;
                record.Price = _Price;
                record.Discount = _Discount;
                //lay thong tin cua file
                string _Photo = "";
                try
                {
                    _Photo = Request.Form.Files[0].FileName;
                }
                catch {; }
                if (!string.IsNullOrEmpty(_Photo))
                {
                    //upload anh moi
                    var timestamp = DateTime.Now.ToFileTime();//chuyen thoi gian ra thanh mot so nguyen
                    _Photo = timestamp + "_" + _Photo;//noi chuoi thoi gian vao bien _Photo
                    //lay duong dan cua file
                    //Path.Combine(duongdan1,duongdan2...) noi duongdan1 va duongdan2... thanh mot duong dan
                    string _Path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Upload/Products", _Photo);
                    //upload file
                    using (var stream = new FileStream(_Path, FileMode.Create))
                    {
                        Request.Form.Files[0].CopyTo(stream);
                    }
                    //cap nhat lai duong dan anh
                    record.Photo = _Photo;
                }
                // xoa categories cu
                var old_categories = db.CategoriesProducts.Where(item => item.ProductId == _id).ToList();
                foreach (var item in old_categories)
                {
                    db.CategoriesProducts.Remove(item);
                    db.SaveChanges();
                }
                // xoa tag cu
                var old_tags = db.TagsProducts.Where(item => item.ProductId == _id).ToList();
                foreach (var item in old_tags)
                {
                    db.TagsProducts.Remove(item);
                    db.SaveChanges();
                }
                // insert du lieu moi vao table CategoriesProducts
                foreach (var item in SelectedCategories)
                {
                    ItemCategoryProduct new_record = new ItemCategoryProduct();
                    new_record.CategoryId = Convert.ToInt32(item);
                    new_record.ProductId = _id;
                    db.CategoriesProducts.Add(new_record);
                    db.SaveChanges();
                }
                // insert du lieu moi vao table TagsProducts
                foreach (var item in SelectedTags)
                {
                    ItemTagProduct new_record = new ItemTagProduct();
                    new_record.TagId = Convert.ToInt32(item);
                    new_record.ProductId = _id;
                    db.TagsProducts.Add(new_record);
                    db.SaveChanges();

                }
                //cap nhat csdl
                db.SaveChanges();
                //---                
            }
            //---
            return RedirectToAction("Index");
        }
        //create
        public IActionResult Create()
        {
            //khai bao bien action de dua vao tham so action cua the form
            ViewBag.action = "/Admin/Products/CreatePost";
            return View("CreateUpdate");
        }
        //create post
        [HttpPost]
        public IActionResult CreatePost(IFormCollection fc)
        {
            //tao mot ban ghi
            ItemProduct record = new ItemProduct();
            //---
            //ham .Trim() de cat khoang trang ben trai va ben phai cua chuoi
            string _Name = fc["Name"].ToString().Trim();//cac dung IFormCollection
            string _Description = Request.Form["Description"].ToString().Trim();//cach dung Request.Form
            string _Content = Request.Form["Content"].ToString().Trim();
            int _Hot = !String.IsNullOrEmpty(Request.Form["Hot"]) ? 1 : 0;
            double _Price = Convert.ToDouble(Request.Form["Price"]);
            double _Discount = Convert.ToDouble(Request.Form["Discount"]);
            var SelectedCategories = Request.Form["Categories"]; // do the selcet la mutiple nen Categories se la 1 array bao gom danh sach cac iid
            var SelectedTags = Request.Form["Tags"];
            if (record != null)
            {
                //---
                record.Name = _Name;
                record.Description = _Description;
                record.Content = _Content;
                record.Hot = _Hot;
                record.Price = _Price;
                record.Discount = _Discount;
                //lay thong tin cua file
                string _Photo = "";
                try
                {
                    _Photo = Request.Form.Files[0].FileName;
                }
                catch {; }
                if (!string.IsNullOrEmpty(_Photo))
                {
                    //upload anh moi
                    var timestamp = DateTime.Now.ToFileTime();//chuyen thoi gian ra thanh mot so nguyen
                    _Photo = timestamp + "_" + _Photo;//noi chuoi thoi gian vao bien _Photo
                    //lay duong dan cua file
                    //Path.Combine(duongdan1,duongdan2...) noi duongdan1 va duongdan2... thanh mot duong dan
                    string _Path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Upload/Products", _Photo);
                    //upload file
                    using (var stream = new FileStream(_Path, FileMode.Create))
                    {
                        Request.Form.Files[0].CopyTo(stream);
                    }
                    //cap nhat lai duong dan anh
                    record.Photo = _Photo;
                }
                //them ban ghi vao table
                db.Products.Add(record);
                //cap nhat csdl
                db.SaveChanges();
                // lay id vua moi insert vao trong table Products
                int InsertedId = record.Id;
                //---
                // insert du lieu moi vao table CategoriesProducts
                foreach (var item in SelectedCategories)
                {
                    ItemCategoryProduct new_record = new ItemCategoryProduct();
                    new_record.CategoryId = Convert.ToInt32(item);
                    new_record.ProductId = InsertedId;
                    db.CategoriesProducts.Add(new_record);
                    db.SaveChanges();
                }
                // insert du lieu moi vao table TagsProducts
                foreach (var item in SelectedTags)
                {
                    ItemTagProduct new_record = new ItemTagProduct();
                    new_record.TagId = Convert.ToInt32(item);
                    new_record.ProductId = InsertedId;
                    db.TagsProducts.Add(new_record);
                    db.SaveChanges();

                }
            }
            //---
            return RedirectToAction("Index");

        }
        //delete
        public IActionResult Delete(int? id)
        {
            int _id = id ?? 0;
            //lay mot ban ghi
            ItemProduct record = db.Products.Where(item => item.Id == _id).FirstOrDefault();
            //xoa ban ghi nay
            db.Products.Remove(record);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
		
	}
}
