using Microsoft.AspNetCore.Mvc;
//thao tác với file thư mục
using System.IO;
// thao tác với file json
using Newtonsoft.Json;
//sử dụng datatable, datarow
using System.Data;
//sử dụng sqlConnection, dataAdapter,...
using System.Data.SqlClient;
//sử dụng các hàm phân trang
using X.PagedList;
//đối tượng mã hóa csdl, gần đối tượng này thành BC
using BC = BCrypt.Net;
using project.Models;// nhận diện các file trong thư mục Models
using Humanizer;
using System.Drawing;


namespace project.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class CustomersController : Controller
	{
		//đối tượng thao tác csdl
		public MyDbContext db = new MyDbContext();
		public IActionResult Index(int? page)
		{
			//khai báo số bản ghi tteen một trang
			int pageSize = 4;
			//tính số trang
			int pageNumber = page ?? 1;//nếu page=null thì PageNumber = 1; nếu Page != null thì PageNumber = page
			List<ItemCustomer> list_Customers = db.Customers.OrderByDescending(item => item.Id).ToList();
			return View(list_Customers.ToPagedList(pageNumber, pageSize));
		}
		//update
		public IActionResult Update(int? id)
		{
			int _id = id ?? 0;
			//khai bao bien action de dua vao tham so action cua the form
			ViewBag.action = "/Admin/Customers/UpdatePost/" + _id;
			//lay mot ban ghi
			var record = db.Customers.Where(item => item.Id == _id).FirstOrDefault();
			return View("CreateUpdate", record);
		}
		//update post
		[HttpPost]
		public IActionResult UpdatePost(int? id, IFormCollection fc)
		{
			int _id = id ?? 0;
			//lay mot ban ghi
			var record = db.Customers.Where(item => item.Id == _id).FirstOrDefault();
			//---
			//ham .Trim() de cat khoang trang ben trai va ben phai cua chuoi
			string _Name = fc["Name"].ToString().Trim();//cac dung IFormCollection
			string _Email = Request.Form["Email"].ToString().Trim();//cach dung Request.Form
			string _Password = Request.Form["Password"].ToString().Trim();
			string _Address = fc["Address"].ToString().Trim();
			string _Phone = fc["Phone"].ToString().Trim();
			if (record != null)
			{
				//---
				//kiem tra xem Email da ton tai chua, neu chua ton tai thi moi cho update
				var check = db.Customers.Where(item => item.Email == _Email && item.Id != _id).FirstOrDefault();
				if (check == null)
				{
					record.Email = _Email;
					record.Name = _Name;
					record.Address = _Address;
					record.Phone = _Phone;
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
					return Redirect("/Admin/Customers/Update/" + _id + "?notify=email-exists");
				//---                
			}
			//---
			return RedirectToAction("Index");
		}
		//create
		public IActionResult Create()
		{
			//khai bao bien action de dua vao tham so action cua the form
			ViewBag.action = "/Admin/Customers/CreatePost/";
			return View("CreateUpdate");
		}
		//create post
		[HttpPost]
		public IActionResult CreatePost(IFormCollection fc)
		{
			string _Name = fc["Name"].ToString().Trim();
			string _Email = fc["Email"].ToString().Trim();
			string _Address = fc["Address"].ToString().Trim();
			string _Phone = fc["Phone"].ToString().Trim();
			string _Password = fc["Password"].ToString().Trim();
			//ma hoa password
			_Password = BC.BCrypt.HashPassword(_Password);
			//kiem tra xem email da ton tai chua, neu chua ton tai thi moi cho insert ban ghi
			//.Count() tra ve ket qua co bao nhieu ban ghi (so)
			var check = db.Customers.Where(item => item.Email == _Email).Count();
			//return Content(check.ToString());
			if (check == 0)
			{
				//khoi tao ban ghi
				ItemCustomer record = new ItemCustomer();
				record.Name = _Name;
				record.Email = _Email;
				record.Password = _Password;
				record.Address = _Address;
				record.Phone = _Phone;
				//them ban ghi
				db.Customers.Add(record);
				//cap nhat thong tin 
				db.SaveChanges();
				return RedirectToAction("Index");
			}
			else
				return Redirect("/Admin/Customers/Create?notify=email-exists");
		}
		//delete
		public IActionResult Delete(int? id)
		{
			int _id = id ?? 0;
			//Lấy một bản ghi
			ItemCustomer record = db.Customers.Where(item => item.Id == _id).FirstOrDefault();
			// xoá bản ghi này
			db.Customers.Remove(record);
			db.SaveChanges();
			return RedirectToAction("Index");
		}
	}
}
