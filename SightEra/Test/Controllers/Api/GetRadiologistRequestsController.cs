using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Test.Data;

namespace Test.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetRadiologistRequestsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public GetRadiologistRequestsController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> getRequests(int id)
        {
            var radiologist = await _context.Radiologists.Include(e=>e.Requests).Include(e=>e.Reports).FirstOrDefaultAsync(e => e.Id == id);
            if (radiologist == null)
                return NotFound();
            var requests = _context.UserRequests.Where(e => e.RadiologistId == id).Select(e => new { id = e.Id,firstName = e.Doctor.FirstName,lastName = e.Doctor.LastName ,  requestDate = e.RequestDate.ToString("dd/MM/yyyy hh:mm tt") , show = e.IsShow});
            return Ok(requests);
        }
    }
}
