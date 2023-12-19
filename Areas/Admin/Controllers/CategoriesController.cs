
using Microsoft.AspNetCore.Mvc;
using System.IO; //thao tac voi file, thu muc
using Newtonsoft.Json;//thao tac voi file json
using System.Data;//su dung DataTalbe, DataRow
using System.Data.SqlClient;//su dung SqlConnection, DataAdapter...
using X.PagedList;//su dung cac ham phan trang
using BC = BCrypt.Net;//doi tuong ma hoa csdl, gan doi tuong nay thanh BC
using project.Models;//nhan dien cac file trong thu muc Models
using System.Security.Cryptography;

// sd ADO
namespace projectnhom14.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoriesController : Controller
    {
        //bien luu chuoi ket noi
        public string strConnectionString;
        //ham tao la ham tu khoi dong dau tien
        public CategoriesController()
        {
            //khoi tao doi tuong co the truy cap duoc vao cac tag cua file appsettings.json
            var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            //doc tag MyConnectionString tai file appsettings.json
            strConnectionString = config.GetConnectionString("MyConnectionString").ToString();
        }
        // truyền dữ liệu ra bên ngoài qua hàm Index
        public IActionResult Index(int? page)
        {
            DataTable dt = new DataTable();
            //khoi tao doi tuong ket noi csdl
            using (SqlConnection conn = new SqlConnection(strConnectionString))
            {
                SqlDataAdapter da = new SqlDataAdapter("select * from Categories where ParentId = 0 order by Id desc", conn);
                //fill ket qua vao DataTable
                da.Fill(dt);
            }
            //khoi tao List de gan du lieu tu DataTable vao List (phuc vu cho viec phan trang su dung X.PageList)
            List<ItemCategory> list_categories = new List<ItemCategory>();
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    list_categories.Add(new ItemCategory() { Id = Convert.ToInt32(dr["Id"]), ParentId = Convert.ToInt32(dr["ParentId"]), Name = dr["Name"].ToString(), DisplayHomePage = Convert.ToInt32(dr["DisplayHomePage"]) });
                }
            }
            //---
            //khai bao so ban ghi tren mot trang
            int pageSize = 4;
            //tinh so trang
            int pageNumber = page ?? 1;
            //---
            return View(list_categories.ToPagedList(pageNumber, pageSize));
        }

        //update
        public IActionResult Update(int? id)
        {
            int _id = id ?? 0;
            //---
            //khoi tao bien action de dua vao thuoc tinh action cua the form
            ViewBag.action = "/Admin/Categories/UpdatePost/" + _id;
            //---            
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(strConnectionString))
            {
                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter("select * from Categories where Id = " + _id, conn);
                /*
                //co the dung cach khac de truy van sql
                SqlCommand cmd = new SqlCommand("select * from Categories where Id = @id", conn);
                //truyen tham so vao chuoi sql
                cmd.Parameters.AddWithValue("@id", _id);
                //thuc thi cau sql, gan vao bien doi tuong da
                SqlDataAdapter da = (SqlDataAdapter)cmd.ExecuteScalar();
                */
                da.Fill(dt);
            }
            //lay mot hang de truyen ra ngoai view
            DataRow dr = dt.Rows.Count > 0 ? dt.Rows[0] : dt.NewRow();
            return View("CreateUpdate", dr);
        }
        [HttpPost]
        public IActionResult UpdatePost(int? id)
        {
            int _id = id ?? 0;
            string _Name = Request.Form["Name"].ToString().Trim();
            int _ParentId = Convert.ToInt32(Request.Form["ParentId"]);
            //neu tick vao o checkbox thi doi tuong name="DisplayHomePage" moi ton tai (neu dung the checkbox)
            int _DisplayHomePage = !String.IsNullOrEmpty(Request.Form["DisplayHomePage"]) ? 1 : 0;
            using (SqlConnection conn = new SqlConnection(strConnectionString))
            {
                //neu dung update, insert, delete (co bien dong du lieu trong database) thi phai mo ket noi
                conn.Open();
                SqlCommand cmd = new SqlCommand("update Categories set Name = @Name, ParentId = @ParentId, DisplayHomePage = @DisplayHomePage where Id = @Id", conn);
                cmd.Parameters.AddWithValue("@Name", _Name);
                cmd.Parameters.AddWithValue("@ParentId", _ParentId);
                cmd.Parameters.AddWithValue("@DisplayHomePage", _DisplayHomePage);
                cmd.Parameters.AddWithValue("@Id", _id);
                //thuc thi cau truy van
                cmd.ExecuteNonQuery();
            }
            return Redirect("/Admin/Categories");
        }
        public IActionResult Create()
        {
            ViewBag.action = "/Admin/Categories/CreatePost";
            return View("CreateUpdate");
        }
        [HttpPost]
        public IActionResult CreatePost()
        {
            string _Name = Request.Form["Name"].ToString().Trim();
            int _ParentId = Convert.ToInt32(Request.Form["ParentId"]);
            //neu tick vao o checkbox thi doi tuong name="DisplayHomePage" moi ton tai (neu dung the checkbox)
            int _DisplayHomePage = !String.IsNullOrEmpty(Request.Form["DisplayHomePage"]) ? 1 : 0;
            //return Content(_DisplayHomePage.ToString());
            using (SqlConnection conn = new SqlConnection(strConnectionString))
            {
                //neu dung update, insert, delete (co bien dong du lieu trong database) thi phai mo ket noi
                conn.Open();
                SqlCommand cmd = new SqlCommand("insert into Categories(Name,ParentId,DisplayHomePage) values(@Name,@ParentId,@DisplayHomePage)", conn);
                cmd.Parameters.AddWithValue("@Name", _Name);
                cmd.Parameters.AddWithValue("@ParentId", _ParentId);
                cmd.Parameters.AddWithValue("@DisplayHomePage", _DisplayHomePage);
                //thuc thi cau truy van
                cmd.ExecuteNonQuery();
            }
            return Redirect("/Admin/Categories");
        }
        public IActionResult Delete(int? id)
        {
            int _id = id ?? 0;
            using (SqlConnection conn = new SqlConnection(strConnectionString))
            {
                //neu dung update, insert, delete (co bien dong du lieu trong database) thi phai mo ket noi
                conn.Open();
                SqlCommand cmd = new SqlCommand("delete from Categories where Id = @Id or ParentId =@Id or ParentId = @Id", conn);
                cmd.Parameters.AddWithValue("@Id", _id);
                //thuc thi cau truy van
                cmd.ExecuteNonQuery();
            }
            return Redirect("/Admin/Categories");
        }
    }
}
