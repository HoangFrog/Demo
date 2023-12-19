using Microsoft.AspNetCore.Mvc;
using System.IO; //thao tac voi file, thu muc
using Newtonsoft.Json;//thao tac voi file json
using System.Data;//su dung DataTalbe, DataRow
using System.Data.SqlClient;//su dung SqlConnection, DataAdapter...
using X.PagedList;//su dung cac ham phan trang
using BC = BCrypt.Net;//doi tuong ma hoa csdl, gan doi tuong nay thanh BC
using project.Models;//nhan dien cac file trong thu muc Models
using System.Security.Cryptography;
using System.Linq;


namespace project.Controllers
{
	public class CartController : Controller
	{
		public MyDbContext db = new MyDbContext();
		public IActionResult Index()
		{
			// khai bao gio hang
			List<Item> shopping_cart = new List<Item>();
			// lay chuoi json luu o session
			string json_cart = HttpContext.Session.GetString("cart");
			if (!string.IsNullOrEmpty(json_cart))
			{
				// convert chuoi json thanh List
				shopping_cart = JsonConvert.DeserializeObject<List<Item>>(json_cart);
			}
			return View(shopping_cart);
		}
		// them san pham vao gio hang
		public IActionResult Buy(int id)
		{
			// goi ham CartAdd de them san pham vao gio hang
			Cart.CartAdd(HttpContext.Session, id);
			return Redirect("/Cart");
		}
		// xoa san pham khoi gio hang
		public IActionResult Remove(int id)
		{
			// goi ham CartRemove de xoa san pham khoi gio hang
			Cart.CartRemove(HttpContext.Session, id);
			return Redirect("/Cart");
		}
		// xoa toan bo gio hang
		public IActionResult Destroy(int id)
		{
			// goi ham CartRemove de xoa toan bo san pham khoi gio hang
			Cart.CartDestroy(HttpContext.Session);
			return Redirect("/Cart/Index?notify=destroy-success");
		}
		// update gio hang
		public IActionResult Update()
		{

			// khai bao gio hang
			List<Item> shopping_cart = new List<Item>();
			// lay chuoi json luu o session
			string json_cart = HttpContext.Session.GetString("cart");
			if (!string.IsNullOrEmpty(json_cart))
			{
				// convert chuoi json thanh List
				shopping_cart = JsonConvert.DeserializeObject<List<Item>>(json_cart);
			}
			// duyet cac item trong shopping_cart de cap nhap lai so luong
			foreach (Item item_cart in shopping_cart)
			{
				string form_name = "product_" + item_cart.ProductRecord.Id;
				int new_quantity = Convert.ToInt32(Request.Form[form_name]);
				// goi ham de update lai so luong
				Cart.CartUpdate(HttpContext.Session, item_cart.ProductRecord.Id, new_quantity);
			}
			return Redirect("/Cart");

		}
		// mua hang
		public IActionResult CheckOut()
		{
			// kiem tra neu chuwa dang nhap thi di chuyen den url dang nhap
			if (String.IsNullOrEmpty(HttpContext.Session.GetString("customer_email")))
			{
				return Redirect("/Account/Login");
			}
			else
			{
				// sau khi mua hang thanh cong thi gio hang se bi xoa di
				Cart.CartCheckOut(HttpContext.Session, Convert.ToInt32(HttpContext.Session.GetString("customer_id")));
				return Redirect("/Cart/Index?notify=checkout-success");
			}
		}
	}
}
