using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Test.Data;
using Test.Models;
using Test.ViewModel;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.IO;
using Microsoft.AspNetCore.SignalR;
using Test.Services;

namespace Test.Controllers
{
    public class RequestController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _user;
        private readonly IHubContext<NotificationHub> _hubContext;
        public RequestController(ApplicationDbContext context, UserManager<ApplicationUser> user, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _user = user;
            _hubContext = hubContext;
        }
        public async Task<IActionResult> AllRequests()
        {
            var user = await _user.GetUserAsync(User);
            var doctor = _context.Doctors.Where(e => e.Email == user.Email).FirstOrDefault();
            TempData["ID"] = doctor.Id;
            var requests = _context.UserRequests.Where(e => e.DoctorId == Convert.ToInt32(TempData["ID"])).Include(n => n.Radiologist).ToList();
            return View(requests);
        }
        public async Task<IActionResult> SendRequest()
        {
            var radiologists = await _context.Radiologists.ToListAsync();
            var fullName = radiologists.Select(e => new { id = e.Id, FullName = e.FirstName + " " + e.LastName });
            ViewBag.Radiologists = new SelectList(fullName, "id", "FullName");
            // ClaimsPrincipal currentUser = this.User;
            var model = new RequestUser();
            //model.DoctorId = doctor.Id;
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendRequest(RequestUser model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var user = await _user.GetUserAsync(User);
            var doctor = await _context.Doctors.Include(e => e.Requests).FirstOrDefaultAsync(e => e.Email == user.Email);
            var radiologist = _context.Radiologists.Include(e => e.Requests).FirstOrDefault(e => e.Id == model.RadiologistId);
            TempData["ID"] = doctor.Id;
            int id = Convert.ToInt32(TempData["ID"]);
            var newModel = new RequestUser()
            {
                FullName = model.FullName,
                Gender = model.Gender,
                PhoneNumber = model.PhoneNumber,
                Age = model.Age,
                RequestDate = DateTime.Now,
                DoctorId = model.DoctorId,
                Doctor = doctor,
                RadiologistId = radiologist.Id,
                Radiologist = radiologist
            };
            doctor.Requests?.Add(newModel);
            radiologist.Requests?.Add(newModel);
            EntityEntry<RequestUser> userRequestEntry = await _context.UserRequests.AddAsync(newModel);
            await _context.SaveChangesAsync();
            var notificationHub = _hubContext.Clients.All;
            await notificationHub.SendAsync("SendNewRequest", "New Request Added");
            return RedirectToAction(nameof(AllRequests));
        }
        public async Task<IActionResult> EditRequest(int RequestId)
        {
            var request = await _context.UserRequests.Where(e => e.Id == RequestId).Include(r => r.Radiologist).FirstOrDefaultAsync();
            var radiologists = await _context.Radiologists.ToListAsync();
            var fullName = radiologists.Select(e => new { id = e.Id, FullName = e.FirstName + " " + e.LastName });
            ViewBag.Radiologists = new SelectList(fullName, "id", "FullName");
            if (request == null)
                return NotFound();
            var model = new RequestUser()
            {
                Id = RequestId,
                FullName = request.FullName,
                Age = request.Age,
                Gender = request.Gender,
                IsShow = request.IsShow,
                RadiologistId = request.RadiologistId,
                RequestDate = request.RequestDate,
                PhoneNumber = request.PhoneNumber,
                Radiologist = request.Radiologist
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRequest(RequestUser newModel)
        {
            if (!ModelState.IsValid)
                return View(newModel);
            var request = _context.UserRequests.AsNoTracking().Where(e => e.Id == newModel.Id).Include(r => r.Radiologist).FirstOrDefault();
            if (request == null)
                return NotFound();
            var user = await _user.GetUserAsync(User);
            var doctor = _context.Doctors.Where(e => e.Email == user.Email).FirstOrDefault();
            var radiologist = _context.Radiologists.FirstOrDefault(e => e.Id == newModel.RadiologistId);
            doctor.Requests?.Remove(newModel);
            radiologist.Requests?.Remove(newModel);
            newModel.Doctor = doctor;
            newModel.DoctorId = doctor.Id;
            newModel.Radiologist = _context.Radiologists.Where(e => e.Id == newModel.RadiologistId).FirstOrDefault();
            newModel.RequestDate = DateTime.Now;
            doctor.Requests?.Add(newModel);
            radiologist.Requests?.Add(newModel);
            EntityEntry entityEntry = _context.Entry(newModel);
            entityEntry.State = EntityState.Modified;
            _context.SaveChanges();
            return RedirectToAction(nameof(AllRequests));
        }
        public async Task<IActionResult> GetRequest(int RequestId)
        {
            TempData["req_id"] = RequestId;
            var request = await _context.UserRequests.Include(e => e.Doctor).Include(e => e.Radiologist).Where(e => e.Id == RequestId).FirstOrDefaultAsync();
            if (request == null)
                return NotFound();
            return View(request);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetRequest()
        {
            var requestId = Convert.ToInt32(TempData["req_id"]);
            var request = await _context.UserRequests.Include(e => e.Doctor).Include(e => e.Radiologist).FirstOrDefaultAsync(e => e.Id == requestId);
            if (request == null)
                return NotFound();
           return RedirectToAction("UploadPhotos");
        }
        public async Task<IActionResult> UploadPhotos(int req)
        {
              var requestId = Convert.ToInt32(TempData["req_id"]);
              var request = await _context.UserRequests.Include(e => e.Doctor).Include(e => e.Radiologist).FirstOrDefaultAsync(e => e.Id == requestId);
              if (request == null)
                  return NotFound();
            TempData["r_id"] = requestId;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadPhotos()
        {
            int requestId = Convert.ToInt32(TempData["r_id"]);
            var request = await _context.UserRequests.Include(e => e.Doctor).Include(e => e.Radiologist).FirstOrDefaultAsync(e => e.Id == requestId);
            if (request == null)
                return NotFound();
            byte[] photo1 = new byte[SByte.MaxValue];
            byte[] photo2 = new byte[SByte.MaxValue];
            string name1 = "";
            string name2 = "";
            if (Request.Form.Files.Count > 0)
            {
                var file1 = Request.Form.Files.FirstOrDefault();
                var file2 = Request.Form.Files.Skip(1).FirstOrDefault();

                name1 = file1.FileName;
                name2 = file2.FileName;
                using (var dataStraem = new MemoryStream())
                {
                    await file1.CopyToAsync(dataStraem);
                    // report.EyePhoto = dataStraem.ToArray();
                    photo1 = dataStraem.ToArray();
                    System.IO.File.WriteAllBytes(name1, photo1);
                    //report.Request.IsShow = true;
                    //_context.SaveChanges();
                }
                using (var dataStraem = new MemoryStream())
                {
                    await file2.CopyToAsync(dataStraem);
                    // report.EyePhoto = dataStraem.ToArray();
                    photo2 = dataStraem.ToArray();
                    System.IO.File.WriteAllBytes(name2, photo2);
                    //report.Request.IsShow = true;
                    //_context.SaveChanges();
                }
            }

             var  report = new ReportUser()
                {
                    Doctor = request.Doctor,
                    DoctorId = request.DoctorId,
                    Radiologist = request.Radiologist,
                    RadiologistId = request.RadiologistId,
                    Request = request,
                    RightEye = photo2,
                    LeftEye = photo1,
                    leftEyeDiseaseName="",
                    rightEyeDiseaseName="",
                    fileName1 = name2,
                    fileName2 = name1,
                    RequestId = requestId
                };
            _context.UserReports.Add(report);
            _context.SaveChanges();
            TempData["rep_id"] = _context.UserReports.Include(e=>e.Request).FirstOrDefault(e=>e.RequestId == requestId).Id;
            return RedirectToAction("CallPredictApi", "GetData");
        }
    }
    }