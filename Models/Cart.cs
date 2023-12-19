using Microsoft.AspNetCore.Http;
//muon su dung thu vien jSon thi phai them dong duoi
using Newtonsoft.Json;

namespace project.Models
{
    public class Cart
    {
        protected static readonly MyDbContext db = new MyDbContext();
        //------        
        public static T GetObjectFromJson<T>(ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
        //------
        //lay gio hang dang ton tai
        public static List<Item> GetCart(ISession session)
        {
            //JsonConvert.DeserializeObject<T>("cart")
            List<Item> cart = Cart.GetObjectFromJson<List<Item>>(session, "cart");
            return cart;
        }
        //add item to cart
        public static void CartAdd(ISession session, int id)
        {
            if (Cart.GetObjectFromJson<List<Item>>(session, "cart") == null)
            {
                List<Item> cart = new List<Item>();
                ItemProduct item = db.Products.Where(tbl => tbl.Id == id).FirstOrDefault();
                cart.Add(new Item { ProductRecord = item, Quantity = 1 });
                session.SetString("cart", JsonConvert.SerializeObject(cart));
            }
            else
            {
                List<Item> cart = Cart.GetObjectFromJson<List<Item>>(session, "cart");

                int index = Cart.isExist(session, id);
                if (index != -1)
                {
                    cart[index].Quantity++;
                }
                else
                {
                    ItemProduct item = db.Products.Where(tbl => tbl.Id == id).FirstOrDefault();
                    cart.Add(new Item { ProductRecord = item, Quantity = 1 });
                }
                session.SetString("cart", JsonConvert.SerializeObject(cart));
            }
        }
        //remove item in cart
        public static void CartRemove(ISession session, int id)
        {
            List<Item> cart = Cart.GetObjectFromJson<List<Item>>(session, "cart");
            int index = isExist(session, id);
            cart.RemoveAt(index);
            session.SetString("cart", JsonConvert.SerializeObject(cart));
        }
        //remove all item in cart
        public static void CartDestroy(ISession session)
        {
            List<Item> cart = new List<Item>();
            session.SetString("cart", JsonConvert.SerializeObject(cart));
        }
        public static void CartUpdate(ISession session, int id, int quantity)
        {
            List<Item> cart = Cart.GetObjectFromJson<List<Item>>(session, "cart");
            //---
            for (int i = 0; i < cart.Count; i++)
            {
                if (cart[i].ProductRecord.Id == id)
                {
                    cart[i].Quantity = quantity;
                }
            }
            //---
            session.SetString("cart", JsonConvert.SerializeObject(cart));
        }
        private static int isExist(ISession session, int id)
        {
            List<Item> cart = Cart.GetObjectFromJson<List<Item>>(session, "cart");
            for (int i = 0; i < cart.Count; i++)
            {
                if (cart[i].ProductRecord.Id == id)
                {
                    return i;
                }
            }
            return -1;
        }
        public static double CartTotal(ISession session)
        {
            List<Item> items_cart = Cart.GetCart(session);
            if (items_cart != null)
            {
                double total = 0;
                foreach (var item in items_cart)
                {
                    total += item.Quantity * (item.ProductRecord.Price - (item.ProductRecord.Price * item.ProductRecord.Discount) / 100);
                }
                return total;
            }
            else
                return 0;
        }
        public static void CartCheckOut(ISession session, int customer_id)
        {
            //khởi tạo đối tượng thao tác csdl
            MyDbContext db = new MyDbContext();
            //---
            List<Item> _cart = Cart.GetCart(session);
            //insert du lieu vao table Orders
            ItemOrder _RecordOrder = new ItemOrder();
            _RecordOrder.CustomerId = customer_id;
            _RecordOrder.Create = DateTime.Now;
            _RecordOrder.Price = _cart.Sum(tbl => tbl.ProductRecord.Price * tbl.Quantity);
            db.Orders.Add(_RecordOrder);
            db.SaveChanges();
            //lay id vua insert
            int order_id = _RecordOrder.Id;
            //duyet cac san pham trong session, moi phan tu se add thanh 1 ban ghi trong OrdersDetail
            foreach (var item in _cart)
            {
                ItemOrderDetail _RecordOrdersDetail = new ItemOrderDetail();
                _RecordOrdersDetail.OrderId = order_id;
                _RecordOrdersDetail.ProductId = item.ProductRecord.Id;
                _RecordOrdersDetail.Price = item.ProductRecord.Price - (item.ProductRecord.Price * item.ProductRecord.Discount) / 100;
                _RecordOrdersDetail.Quantity = item.Quantity;
                //---
                db.OrdersDetail.Add(_RecordOrdersDetail);
                db.SaveChanges();
            }
            //xoa tat cac cac phan tu trong gio hang
            Cart.CartDestroy(session);
        }
        //lấy số sản phẩm trong giỏ hàng
        public static int CartQuantity(ISession session)
        {
            List<Item> items_cart = Cart.GetCart(session);
            if (items_cart != null)
            {
                return items_cart.Count;
            }
            else
                return 0;
        }
    }
}
