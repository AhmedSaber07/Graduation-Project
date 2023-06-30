using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Test.Data;
using Test.Models;

namespace Test.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public string Username { get; set; }
        public string Usertype { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [RegularExpression("^[A-Za-z]+$", ErrorMessage = "Please Enter Valid Name")]
            [Required]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }
            [RegularExpression("^[A-Za-z]+$", ErrorMessage = "Please Enter Valid Name")]
            [Required]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Display(Name = "Profile Picture")]
            public byte[] ProfilePicture { get; set; }
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var userType = user.UserType;
                // var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;
            Usertype = userType;
            Input = new InputModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                ProfilePicture = user.ProfilePicture
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            //var user_context;
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            if (user.UserType == "Doctor")
            {
                var user_context = _context.Doctors.FirstOrDefault(e => e.UserName == user.UserName);
            }
            else
            {
                var user_context = _context.Radiologists.FirstOrDefault(e => e.UserName == user.UserName);
            }
            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
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
            var firstName = user.FirstName;
            var lastName = user.LastName;

            if(Input.FirstName!=firstName)
            {
                user.FirstName = Input.FirstName;
                await _userManager.UpdateAsync(user);
            }

            if (Input.LastName != lastName)
            {
                user.LastName = Input.LastName;
                await _userManager.UpdateAsync(user);
            }
            if (Request.Form.Files.Count > 0)
            {
                var file = Request.Form.Files.FirstOrDefault();
                using (var dataStraem = new MemoryStream())
                {
                    await file.CopyToAsync(dataStraem);
                    user.ProfilePicture = dataStraem.ToArray();
                }
                await _userManager.UpdateAsync(user);
            }

            if (user.UserType == "Doctor")
            {
                var user_context = _context.Doctors.FirstOrDefault(e => e.UserName == user.UserName);
                if (Input.FirstName != user_context.FirstName)
                {
                    user_context.FirstName = Input.FirstName;
                    EntityEntry entityEntry = _context.Entry(user_context);
                    entityEntry.State = EntityState.Modified;
                    _context.SaveChanges();
                }

                if (Input.LastName != user_context.LastName)
                {
                    user_context.LastName = Input.LastName;
                    EntityEntry entityEntry = _context.Entry(user_context);
                    entityEntry.State = EntityState.Modified;
                    _context.SaveChanges();
                }
                if (Request.Form.Files.Count > 0)
                {
                    var file = Request.Form.Files.FirstOrDefault();
                    using (var dataStraem = new MemoryStream())
                    {
                        await file.CopyToAsync(dataStraem);
                        user_context.ProfilePicture = dataStraem.ToArray();
                    }
                    EntityEntry entityEntry = _context.Entry(user_context);
                    entityEntry.State = EntityState.Modified;
                    _context.SaveChanges();
                }
            }
            else
            {
                var user_context = _context.Radiologists.FirstOrDefault(e => e.UserName == user.UserName);
                if (Input.FirstName != user_context.FirstName)
                {
                    user_context.FirstName = Input.FirstName;
                    EntityEntry entityEntry = _context.Entry(user_context);
                    entityEntry.State = EntityState.Modified;
                    _context.SaveChanges();
                }

                if (Input.LastName != user_context.LastName)
                {
                    user_context.LastName = Input.LastName;
                    EntityEntry entityEntry = _context.Entry(user_context);
                    entityEntry.State = EntityState.Modified;
                    _context.SaveChanges();
                }
                if (Request.Form.Files.Count > 0)
                {
                    var file = Request.Form.Files.FirstOrDefault();
                    using (var dataStraem = new MemoryStream())
                    {
                        await file.CopyToAsync(dataStraem);
                        user_context.ProfilePicture = dataStraem.ToArray();
                    }
                    EntityEntry entityEntry = _context.Entry(user_context);
                    entityEntry.State = EntityState.Modified;
                    _context.SaveChanges();
                }
            }
            //var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            //if (Input.PhoneNumber != phoneNumber)
            //{
            //    var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
            //    if (!setPhoneResult.Succeeded)
            //    {
            //        StatusMessage = "Unexpected error when trying to set phone number.";
            //        return RedirectToPage();
            //    }
            //}

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
