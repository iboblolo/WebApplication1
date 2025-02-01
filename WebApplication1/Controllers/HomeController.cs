using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplication1.Models;
using WebApplication1.Data;
using WebApplication1.DbModels;
using Microsoft.EntityFrameworkCore;
using WebApplication1.ViewModels;
using System.Text.Json;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using System.Net.Mail;
using System.Net;

namespace WebApplication1.Controllers
{
    static public class DateMem
    {
        public static DateOnly ToDateOnly(this DateTime datetime)
   => DateOnly.FromDateTime(datetime);
    }


    public class HomeController : Controller
    {
        
        ApplicationDbContext db;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ApplicationDbContext context, 
            ILogger<HomeController> logger)
        {
            db = context;
            _logger = logger;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            
            ProductsViewModel productsViewModel = new ProductsViewModel();
            productsViewModel.Products = await db.Products.ToListAsync();
            return View(productsViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Index(string search)
        {

            ProductsViewModel productsViewModel = new ProductsViewModel();
            productsViewModel.Products = await db.Products.Where(p => p.ProductName.ToLower().Contains(search)).ToListAsync();
            return View(productsViewModel);
        }

        [Authorize]
        public async Task<IActionResult> Wishlist()
        {

            WishlistViewModel viewModel = new WishlistViewModel();
            List<WishlistItem> wishlistItems = new List<WishlistItem>();

            string name = this.User.Identity.Name;
            string userId = db.Users.Where(u => u.UserName == name).First().Id;

            List<Wishlist> wishlists = db.Wishlists.Where(w => w.UserId == userId).ToList();

            foreach (Wishlist wl in wishlists)
            {
                WishlistItem item = new WishlistItem()
                {
                    ProductName = db.Products.Where(p => p.Id == wl.ProductId).First().ProductName,
                    Price = db.Products.Where(p => p.Id == wl.ProductId).First().Price,
                    Image = db.Products.Where(p => p.Id == wl.ProductId).First().Image,
                    Description = db.Products.Where(p => p.Id == wl.ProductId).First().Description,
                    WishlistID = wl.Id,
                    ProductId = wl.ProductId
                };
                wishlistItems.Add(item);
            }
            viewModel.Wishlist = wishlistItems;

            return View(viewModel);
        }

        [Authorize]
        [HttpPost]
        public void AddingToCart(string json)
        {
            var jsondata = JsonConvert.DeserializeObject<AddToCartModel>(json);
            string name = jsondata.Name;
            int ProductID = jsondata.ID;
            Cart cart = new Cart();
            int productId = jsondata.ID;
            string userId = db.Users.Where(u => u.UserName == name).First().Id;
            if (db.Carts.Where(c => c.UserId == userId && c.ProductId == productId).FirstOrDefault() != null)
            {
                db.Carts.Where(c => c.UserId == userId && c.ProductId == productId).First().Count++;
            }
            else 
            {
                cart.Count = 1;
                cart.UserId = userId;
                cart.ProductId = productId;
                db.Carts.Add(cart);
            }
            db.SaveChanges();
        }

        

        [HttpPost]
        public async void AddressDelete(string json)
        {
            
            var jsondata = JsonConvert.DeserializeObject<StringObject>(json);
            string adrName = jsondata.comm;      

            db.Addresses.Remove(db.Addresses.Where(a => a.Address == adrName).First());
            db.SaveChanges();

        }

        [HttpPost]
        public async void WayDelete(string json)
        {

            var jsondata = JsonConvert.DeserializeObject<StringObject>(json);
            string adrName = jsondata.comm;

            db.WaysToPay.Remove(db.WaysToPay.Where(a => a.WayToPay == adrName).First());
            db.SaveChanges();

        }

        [Authorize]
        [HttpPost]
        public void AddingToWishlist(string json)
        {
            var jsondata = JsonConvert.DeserializeObject<IdObject>(json);
            Wishlist wishlist = new Wishlist();
            int productId = jsondata.Id;
            string name = this.User.Identity.Name;
            string userId = db.Users.Where(u => u.UserName == name).First().Id;
            wishlist.UserId = userId;
            wishlist.ProductId = productId;
            if(db.Wishlists.Where(w => w.UserId == wishlist.UserId && w.ProductId == wishlist.ProductId).
                FirstOrDefault() == null)
            {
                db.Wishlists.Add(wishlist);
                db.SaveChanges();
            }
        }
        [HttpPost]
        public void CountInc(string json)
        {
            var jsondata = JsonConvert.DeserializeObject<IdObject>(json);
            int cartID = jsondata.Id;
            db.Carts.Where(c => c.Id == cartID).First().Count++;
            db.SaveChanges();
        }
        [HttpPost]
        public void CountDec(string json)
        {
            var jsondata = JsonConvert.DeserializeObject<IdObject>(json);
            int cartID = jsondata.Id;
            db.Carts.Where(c => c.Id == cartID).First().Count--;
            db.SaveChanges();
        }
        [HttpPost]
        public void CartDelete(string json)
        {
            var jsondata = JsonConvert.DeserializeObject<IdObject>(json);
            int cartID = jsondata.Id;
            db.Carts.Remove(db.Carts.Where(c => c.Id == cartID).First());
            db.SaveChanges();
        }
        [HttpPost]
        public void WishDelete(string json)
        {
            var jsondata = JsonConvert.DeserializeObject<IdObject>(json);
            int wishID = jsondata.Id;
            db.Wishlists.Remove(db.Wishlists.Where(w => w.Id == wishID).First());
            db.SaveChanges();
        }

       

        [HttpPost]
        public void AddOrder(string json)
        {
            var jsondata = JsonConvert.DeserializeObject<StringObject>(json);
            string comment = jsondata.comm;

            string name = this.User.Identity.Name;
            string userId = db.Users.Where(u => u.UserName == name).First().Id;
            Order order = new Order();
            order.UserId = userId;
            order.Comment = comment;
            order.Date = DateTime.Now;
            List<Cart> carts = db.Carts.Where(c => c.UserId == userId && !c.IsOrdered).ToList();
            List<CartToOrder> cartsToOrder = new List<CartToOrder>();
            decimal summ = 0;
            foreach (Cart cart in carts)
            {
                summ += cart.Count * db.Products.Where(p => p.Id == cart.ProductId).First().Price;
            }

            order.Cost = summ;

            db.Orders.Add(order);
            db.SaveChanges();


            MailAddress from = new MailAddress("samoilov0505@bk.ru", "КомпКо");
            MailAddress to = new MailAddress(db.Users.Where(u => u.UserName == this.User.Identity.Name).First().Email);

            MailMessage milo = new MailMessage(from, to);

            milo.Subject = "Покупка в КомпКо";

            milo.Body = string.Format("На данный email была совершена покупка в магазине КомпКо, на сумму {0}, в следующее время {1}", summ, order.Date);
            milo.IsBodyHtml = false;
            SmtpClient smtp = new SmtpClient("smtp.bk.ru", 587);

            smtp.Credentials = new NetworkCredential("samoilov0505@bk.ru", "RgpH2tjHgYBab9tqBjuV");
            smtp.EnableSsl = true;
            smtp.Send(milo);

            int orderId = order.Id;

            foreach (Cart cart in carts)
            {
                cartsToOrder.Add(new CartToOrder { OrderId = orderId, CartId = cart.Id});
                cart.IsOrdered = true;
                db.Carts.Update(cart);
            }
            db.CartToOrders.AddRange(cartsToOrder);
            db.SaveChanges();

        }
        public IActionResult Privacy()
        {
            return View();
        }
        [Authorize]
        public async Task<IActionResult> Cart()
        {
            CartViewModel model = new CartViewModel();
            
            

            List<Cart> carts = await db.Carts.ToListAsync();
            List<CartItem> items = new List<CartItem>();
            foreach (Cart cart in carts)
            {
                if (cart.UserId == db.Users.Where(u => u.UserName == this.User.Identity.Name).First().Id && cart.IsOrdered == false)
                {
                    CartItem item = new CartItem()
                    {
                        ProductName = db.Products.Where(p => p.Id == cart.ProductId).First().ProductName,
                        Price = db.Products.Where(p => p.Id == cart.ProductId).First().Price,
                        Count = cart.Count,
                        Image = db.Products.Where(p => p.Id == cart.ProductId).First().Image,
                        CartID = cart.Id,
                    };
                    items.Add(item);
                }
            }

            model.Addresses = db.Addresses.Where(a => a.UserId ==
            db.Users.Where(u => u.UserName == this.User.Identity.Name).First().Id).Select(a => a.Address).ToList();

            model.WaysToPay = db.WaysToPay.Where(a => a.UserId ==
            db.Users.Where(u => u.UserName == this.User.Identity.Name).First().Id).Select(a => a.WayToPay).ToList();

           

            model.CartItems = items;
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
