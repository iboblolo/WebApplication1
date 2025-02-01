using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using WebApplication1.Data;
using WebApplication1.DbModels;

namespace WebApplication1.Areas.Identity.Pages.Account.Manage
{
    public class WaysToPayModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        ApplicationDbContext db;
        public List<string> WaysToPay;

        [BindProperty]
        public InputModel Input { get; set; }

        public WaysToPayModel(
             ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            db = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public class InputModel
        {

            [Required]
            [Display(Name = "Новый способ оплаты")]
            public string NewWayToPay { get; set; }
        }

        private async Task LoadAsync(IdentityUser user)
        {
            Input = new InputModel
            {
                NewWayToPay = "",
            };

        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            WaysToPay = db.WaysToPay.Where(x => x.UserId == user.Id).Select(a => a.WayToPay).ToList();


            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAddWayToPayAsync()
        {
            Console.WriteLine("Сюда дошли");
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }


            if (Input.NewWayToPay != "")
            {
                db.WaysToPay.Add(new WaysToPay() { UserId = user.Id, WayToPay = Input.NewWayToPay });
                db.SaveChanges();
            }
            return RedirectToPage();
        }
    }
}
