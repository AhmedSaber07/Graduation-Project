using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Test.Data;
using Test.Models;

namespace Test.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="Admin")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _user;
        private readonly ApplicationDbContext _context;
        public UsersController(UserManager<ApplicationUser> user, ApplicationDbContext context)
        {
            _user = user;
            _context = context;
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(string UserName)
        {
            var user = await _user.FindByNameAsync(UserName);
            if (user == null)
                return NotFound();
            var result = await _user.DeleteAsync(user);
            if (user.UserType == "Doctor")
            {
                var doctor = _context.Doctors.FirstOrDefault(e => e.UserName == UserName);
                _context.Doctors.Remove(doctor);
            }
            else
            {
                var radiologist = _context.Radiologists.FirstOrDefault(e => e.UserName == UserName);
                _context.Radiologists.Remove(radiologist);
            }
                if (!result.Succeeded)
                throw new Exception();
            _context.SaveChanges();
            return Ok();
        }
    }
}
