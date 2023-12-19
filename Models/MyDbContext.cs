using Microsoft.EntityFrameworkCore; //de ke thua class DbContext

using project.Models;
using System.IO; //de thao tac voi file, thu muc
namespace project.Models
{
    public class MyDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //khoi tao doi tuong co the truy cap duoc vao cac tag cua file appsettings.json
            var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            //doc tag MyConnectionString tai file appsettings.json
            string strConnectionString = config.GetConnectionString("MyConnectionString").ToString();
            //ket noi csdl
            optionsBuilder.UseSqlServer(strConnectionString);
        }

        public DbSet<ItemUser> Users { get; set; } //<=> table Users
        public DbSet<ItemCategory> Categories { get; set; } //<=> table Categories

        public DbSet<ItemAdv> Adv { get; set; }
        public DbSet<ItemCustomer> Customers { get; set; }
        public DbSet<ItemNews> News { get; set; }
        public DbSet<ItemOrder> Orders { get; set; }
        public DbSet<ItemOrderDetail> OrdersDetail { get; set; }
        public DbSet<ItemProduct> Products { get; set; }
        public DbSet<ItemCategoryProduct> CategoriesProducts { get; set; }

        public DbSet<ItemRating> Rating { get; set; }
        public DbSet<ItemSlide> Slides { get; set; }
        public DbSet<ItemTag> Tags { get; set; }
        public DbSet<ItemTagProduct> TagsProducts { get; set; }
        public DbSet<ItemGroup> Groups { get; set; } //<=> table PermissionGroups
        public DbSet<ItemMenu> Menus { get; set; } //<=> table Menus
        public DbSet<ItemUserGroup> UsersGroups { get; set; }
        public DbSet<ItemMenuGroup> MenusGroups { get; set; }

    }
}
