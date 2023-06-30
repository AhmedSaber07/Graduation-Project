using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Test.Data;
using Test.Services;

namespace Test.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Doctor")]
    public class DeleteRequestController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;
        public DeleteRequestController(ApplicationDbContext context, IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
            _context = context;
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteRequest(int Id)
        {
            var request = _context.UserRequests.Where(e => e.Id == Id).FirstOrDefault();
            if (request == null)
                return NotFound();
            var result = _context.UserRequests.Remove(request);
            var doctor = _context.Doctors.FirstOrDefault(e => e.Id == request.DoctorId);
            var radiologist = _context.Radiologists.FirstOrDefault(e => e.Id == request.RadiologistId);
            doctor.Requests.Remove(request);
            radiologist.Requests.Remove(request);
            _context.SaveChanges();
            var notificationHub = _hubContext.Clients.All;
            await notificationHub.SendAsync("DeleteRequest", "Request are Deleted");
            return Ok();
        }
    }
}
