using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Castle.Core.Smtp;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Shared;
using Newtonsoft.Json;
using WebApplication1.Data;
using WebApplication1.DbModels;
using WebApplication1.Models;

namespace WebApplication1.Areas.Identity.Pages.Account.Manage
{
    public class AddressesModel :PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        ApplicationDbContext db;
        public List<string> Addresses;

        [BindProperty]
        public InputModel Input { get; set; }

        public AddressesModel(
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
            [Display(Name = "Новый адрес")]
            public string NewAddress { get; set; }
        }

        private async Task LoadAsync(IdentityUser user)
        {
            Input = new InputModel
            {
                NewAddress = "",
            };
            
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            Addresses = db.Addresses.Where(x => x.UserId == user.Id).Select(a => a.Address).ToList();


            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAddAddressAsync()
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


            if (Input.NewAddress != "")
            {
                db.Addresses.Add(new Addresses() { UserId = user.Id, Address = Input.NewAddress});
                db.SaveChanges();
            }
            return RedirectToPage();
        }
        
    }
}
