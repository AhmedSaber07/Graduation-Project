using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Test.Data;

namespace Test.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetDoctorReportsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public GetDoctorReportsController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> getReports(int id)
        {
            var doctor = await _context.Doctors.Include(e => e.Requests).Include(e => e.Reports).FirstOrDefaultAsync(e => e.Id == id);
            if (doctor == null)
                return NotFound();
            var reports = _context.UserReports.Where(e => e.DoctorId == id).Select(e => new { id = e.Id, firstName = e.Radiologist.FirstName, lastName = e.Radiologist.LastName, reportDate = e.Date.ToString("dd/MM/yyyy hh:mm tt") });
            return Ok(reports);
        }
    }
}
