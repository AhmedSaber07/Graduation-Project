using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Test.Data;
using Test.Models;
using Test.ViewModel;

namespace Test.Controllers
{
    [Authorize(Roles ="Admin")]
    public class ManageController : Controller
    {
        private readonly UserManager<ApplicationUser>_user;
        private readonly ApplicationDbContext _context;
        public ManageController(UserManager<ApplicationUser> user, ApplicationDbContext context)
        {
            _user = user;
            _context = context;
        }
        public IActionResult AddDoctor()
        {
            var viewModel = new AddUserViewModel();
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddDoctor(AddUserViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);
            var emailExists = await _user.FindByEmailAsync(viewModel.Email);
            if (emailExists != null)
            {
                ModelState.AddModelError("Email", "Email is already exists");
                return View(viewModel);
            }
            var user = new ApplicationUser
            {
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
                Email = viewModel.Email,
                UserName = new MailAddress(viewModel.Email).User,
                UserType = viewModel.UserType
            };
            if (Request.Form.Files.Count > 0)
            {
                var file = Request.Form.Files.FirstOrDefault();
                using (var dataStraem = new MemoryStream())
                {
                    await file.CopyToAsync(dataStraem);
                    user.ProfilePicture = dataStraem.ToArray();
                }
            }
            var doctor = new Doctor()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                UserName = user.UserName,
                ProfilePicture = user.ProfilePicture
            };
            await _context.Doctors.AddAsync(doctor);
            var result = await _user.CreateAsync(user, viewModel.Password);
            if (result.Succeeded)
            {
                await _user.AddToRoleAsync(user, viewModel.UserType);
            }
            return RedirectToAction(nameof(ManageDoctors));
        }
        public IActionResult AddRadiologist()
        {
            var viewModel = new AddUserViewModel();
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRadiologist(AddUserViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);
            var emailExists = await _user.FindByEmailAsync(viewModel.Email);
            if(emailExists!=null)
            {
                ModelState.AddModelError("Email", "Email is already exists");
                return View(viewModel);
            }
            var user = new ApplicationUser
            {
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
                Email = viewModel.Email,
                UserName = new MailAddress(viewModel.Email).User,
                UserType = viewModel.UserType
            };
            if (Request.Form.Files.Count > 0)
            {
                var file = Request.Form.Files.FirstOrDefault();
                using (var dataStraem = new MemoryStream())
                {
                    await file.CopyToAsync(dataStraem);
                    user.ProfilePicture = dataStraem.ToArray();
                }
            }
            var radiologist = new Radiologist()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                UserName = user.UserName,
                ProfilePicture = user.ProfilePicture
            };
            await _context.Radiologists.AddAsync(radiologist);
            var result = await _user.CreateAsync(user, viewModel.Password);
            if(result.Succeeded)
            {
               await _user.AddToRoleAsync(user, viewModel.UserType);
            }
            return RedirectToAction(nameof(ManageRadiologists));
        }

        public async Task<IActionResult> EditDoctor(string Email)
        {
            var user = await _user.FindByEmailAsync(Email);
            if (user == null)
                return NotFound();
            var viewModel = new EditUserViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                UserType = user.UserType,
                ProfilePicture = user.ProfilePicture
            };
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDoctor( EditUserViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);
            var user = await _user.FindByEmailAsync(viewModel.Email);
            if (user == null)
                return NotFound();
            var doctor = await _context.Doctors.FirstOrDefaultAsync(e => e.Email == viewModel.Email);

            user.FirstName = viewModel.FirstName;
            user.LastName = viewModel.LastName;
            doctor.FirstName = viewModel.FirstName;
            doctor.LastName = viewModel.LastName;
            if (Request.Form.Files.Count > 0)
            {
                var file = Request.Form.Files.FirstOrDefault();
                using (var dataStraem = new MemoryStream())
                {
                    await file.CopyToAsync(dataStraem);
                    user.ProfilePicture = dataStraem.ToArray();
                    doctor.ProfilePicture = dataStraem.ToArray();
                }
            }
            await _user.UpdateAsync(user);
            EntityEntry entityEntry = _context.Entry(doctor);
            entityEntry.State = EntityState.Modified;
            _context.SaveChanges();
            return RedirectToAction(nameof(ManageDoctors));
        }
        public async Task<IActionResult> EditRadiologist(string Email)
        {
            var user = await _user.FindByEmailAsync(Email);
            if (user == null)
                return NotFound();
            var viewModel = new EditUserViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                UserType = user.UserType,
                ProfilePicture = user.ProfilePicture
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRadiologist( EditUserViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);
            var user = await _user.FindByEmailAsync(viewModel.Email);
            if (user == null)
                return NotFound();
            var radiologist = await _context.Radiologists.FirstOrDefaultAsync(e => e.Email == viewModel.Email);
            user.FirstName = viewModel.FirstName;
            user.LastName = viewModel.LastName;
            radiologist.FirstName = viewModel.FirstName;
            radiologist.LastName = viewModel.LastName;
            if (Request.Form.Files.Count > 0)
            {
                var file = Request.Form.Files.FirstOrDefault();
                using (var dataStraem = new MemoryStream())
                {
                    await file.CopyToAsync(dataStraem);
                    user.ProfilePicture = dataStraem.ToArray();
                    radiologist.ProfilePicture = dataStraem.ToArray();
                }
            }
            await _user.UpdateAsync(user);
            EntityEntry entityEntry = _context.Entry(radiologist);
            entityEntry.State = EntityState.Modified;
            _context.SaveChanges();
            return RedirectToAction(nameof(ManageRadiologists));
        }
        public async Task<IActionResult> ManageDoctors()
        {
            var doctors = await _context.Doctors.Include(e=>e.Requests).Include(e=>e.Reports).ToListAsync();
            return View(doctors);
        }
        public async Task<IActionResult> ManageRadiologists()
        {
            var radiologists = await _context.Radiologists.Include(e=>e.Requests).Include(e=>e.Reports).ToListAsync();
            return View(radiologists);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SearchDoctor(string searchString)
        {
            var doctors = await _context.Doctors.Include(e => e.Requests).Include(e=>e.Reports).ToListAsync();
            if(!string.IsNullOrEmpty(searchString))
            {
                var result = doctors.Where(n => n.FirstName.ToLower().Contains(searchString.ToLower())||n.LastName.ToLower().Contains(searchString.ToLower())).ToList();
                return View("ManageDoctors",result);
            }
            return View("ManageDoctors", doctors);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SearchRadiologist(string searchString)
        {
            var radiologists = await _context.Radiologists.Include(e => e.Requests).Include(e=>e.Reports).ToListAsync();
            if (!string.IsNullOrEmpty(searchString))
            {
                var result = radiologists.Where(n => n.FirstName.ToLower().Contains(searchString.ToLower()) || n.LastName.ToLower().Contains(searchString.ToLower())).ToList();
                return View("ManageRadiologists", result);
            }
            return View("ManageRadiologists", radiologists);
        }
    }
}
