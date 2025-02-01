#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Areas.Identity.Pages.Account.Manage
{
    public class OrdersModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        ApplicationDbContext db;

        public List<OrderItem> Orders;


        public OrdersModel(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            db = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            string name = user.UserName;
            string uId = db.Users.Where(u => u.UserName == name).First().Id;
            List<OrderItem> orderItems = new List<OrderItem>();
            List<string> coms = db.Orders.Where(x => x.UserId == uId).Select(x => x.Comment).ToList();
            List<decimal> costs = db.Orders.Where(x => x.UserId == uId).Select(x => x.Cost).ToList();
            List<DateTime?> dates = db.Orders.Where(x => x.UserId == uId).Select(x => x.Date).ToList();

            for (int i = 0; i < coms.Count; i++)
            {
                orderItems.Add(new OrderItem() { Comment = coms[i], Cost = costs[i] , Date = dates[i] });
            }

            Orders = orderItems;
            db.SaveChanges();

            return Page();
        }

    }
}
