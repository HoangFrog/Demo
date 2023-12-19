
using Microsoft.AspNetCore.Mvc;
using System.IO; //thao tac voi file, thu muc
using Newtonsoft.Json;//thao tac voi file json
using System.Data;//su dung DataTalbe, DataRow
using System.Data.SqlClient;//su dung SqlConnection, DataAdapter...
using X.PagedList;//su dung cac ham phan trang
using BC = BCrypt.Net;//doi tuong ma hoa csdl, gan doi tuong nay thanh BC
using project.Models;//nhan dien cac file trong thu muc Models
using System.Security.Cryptography;

namespace project.Controllers
{
	public class ProductsController : Controller
	{
		public MyDbContext db = new MyDbContext();
		public IActionResult Category(int? id , int? page)
		{
			int _CategoryId = id ?? 0;
			//tao bien de truyen ra ngoai view
			ViewBag.CategoryId = _CategoryId;
			//khai bao so ban ghi tren mot trang
			int pageSize = 9;
			//tinh so trang
			int pageNumber = page ?? 1;
			List<ItemProduct> list_record = (from item in db.Products orderby item.Id descending select item).ToList();
			// neu id > 0 thi thuc hien where -> co the truy van linq tiep o bien list_record
			if (_CategoryId > 0)
			{
				list_record = (from item_product in list_record join item_category_product in db.CategoriesProducts on item_product.Id equals item_category_product.ProductId join item_category in db.Categories on item_category_product.CategoryId equals item_category.Id where item_category.Id == _CategoryId select item_product).ToList();
			}

			// sắp xếp
			// lay bien order truyen tu url
			string order = "";
			if (!String.IsNullOrEmpty(Request.Query["order"]))
			{
				order = Request.Query["order"];
			}
			// truyen bien order ra ngoai view de thuc hien selected dropdown
			ViewBag.Order = order;
			switch (order)
			{
				case "name-asc":
					list_record = list_record.OrderBy(item => item.Id).ToList();
					break;
				case "name-desc":
					list_record = list_record.OrderByDescending(item => item.Id).ToList();
					break;
				case "price-asc":
					list_record = list_record.OrderBy(item => item.Price).ToList();
					break;
				case "price-desc":
					list_record = list_record.OrderByDescending(item => item.Price).ToList();
					break;
			}
			return View(list_record.ToPagedList(pageNumber, pageSize));
		}
		// chi tiết sản phẩm
		public IActionResult Detail(int? id)
		{
			int _id = id ?? 0;
			// lay mot ban ghi
			ItemProduct item_product = db.Products.Where(item => item.Id == _id).FirstOrDefault();
			return View(item_product);
		}
		
		public IActionResult SearchPrice(int? page)
		{
			int fromPrice = Convert.ToInt32(Request.Query["fromPrice"]);
			int toPrice = Convert.ToInt32(Request.Query["toPrice"]);
			//khai bao so ban ghi tren mot trang
			int pageSize = 6;
			//tinh so trang
			int pageNumber = page ?? 1;
			//---
			ViewBag.fromPrice = fromPrice;
			ViewBag.toPrice = toPrice;
			//---
			List<ItemProduct> list_products = db.Products.Where(item => item.Price >= fromPrice && item.Price <= toPrice).OrderByDescending(item => item.Id).ToList();
			return View("SearchPrice", list_products.ToPagedList(pageNumber, pageSize));
		}
		public IActionResult Tag(int? id, int? page)
		{
			int tag_id = id ?? 0;
			ViewBag.tag_id = tag_id;
			//---
			//số bản ghi trên một trang
			int pageSize = 6;
			//số trang
			int pageNumber = page ?? 1;
			List<ItemProduct> list_products = (from item_tag in db.Tags join item_tag_product in db.TagsProducts on item_tag.Id equals item_tag_product.TagId join item_product in db.Products on item_tag_product.ProductId equals item_product.Id where item_tag.Id == tag_id select item_product).ToList();
			return View("Tag", list_products.ToPagedList(pageNumber, pageSize));
		}
		public IActionResult AJaxSearch()
		{
			string key = "";
			if (!String.IsNullOrEmpty(Request.Query["key"]))
				key = Request.Query["key"];
			List<ItemProduct> list_products = db.Products.Where(item => item.Name.Contains(key)).ToList();
			string str = "";
			foreach (var item in list_products)
			{
				str += "<li><a href='/Products/Detail/"+item.Id+"'><img src='/Upload/Products/" + item.Photo + "'>" + item.Name + "</a></li>";
			}
			return Content(str);
		}
		public IActionResult SearchName(int? page)
		{
			//khi lấy biến từ url thì mặc định biến này sẽ là kiểu string -> nếu là số thì cần convert nó
			string key = "";
			if (!String.IsNullOrEmpty(Request.Query["key"]))
				key = Request.Query["key"];
			//---
			//tạo 2 biến để đưa giá ra ngoài view
			ViewBag.key = key;
			//---
			//số bản ghi trên một trang
			int pageSize = 6;
			//số trang
			int pageNumber = page ?? 1;
			List<ItemProduct> list_record = db.Products.Where(item => item.Name.Contains(key)).ToList();
			return View("SearchName", list_record.ToPagedList(pageNumber, pageSize));
		}
		//danh so sao san pham
		public IActionResult Rating(int? id)
		{
			int _id = id ?? 0;
			int star = Convert.ToInt32(Request.Query["star"]);
			//insert ban ghi vao table Rating
			ItemRating record = new ItemRating();
			record.ProductId = _id;
			record.Star = star;
			//---
			db.Add(record);
			db.SaveChanges();
			//---
			return Redirect("/Products/Detail/" + _id);
		}
	}
}
