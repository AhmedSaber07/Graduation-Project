using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Test.Data;
using Test.Models;

namespace Test.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfirmRequestController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ConfirmRequestController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpPost]
        public async Task<IActionResult> Confirm(int requestId)
        {
            var request = await _context.UserRequests.Include(e => e.Radiologist).FirstOrDefaultAsync(e => e.Id == requestId);
            if (request == null)
                return NotFound();
            if (!ModelState.IsValid)
                return NotFound();
            var radiologist = await _context.Radiologists.FirstOrDefaultAsync(e => e.Id == request.RadiologistId);
            if (Request.Form.Files.Count > 0)
            {
                var file = Request.Form.Files.FirstOrDefault();
                using (var dataStraem = new MemoryStream())
                {
                    await file.CopyToAsync(dataStraem);
                    //radiologist.EyePhoto = dataStraem.ToArray();
                }
            }
            else
                return NotFound();
            if (request.IsShow)
                return Ok();
            request.IsShow = true;
            EntityEntry entityEntry = _context.Entry(request);
            entityEntry.State = EntityState.Modified;
            EntityEntry entityEntry2 = _context.Entry(radiologist);
            entityEntry2.State = EntityState.Modified;
           await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
