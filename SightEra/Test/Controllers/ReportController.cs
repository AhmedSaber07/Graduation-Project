using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Test.Data;
using Test.Models;
using AspNetCore.Reporting;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.AspNetCore.SignalR;
using Test.Services;

namespace Test.Controllers
{
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IWebHostEnvironment _iwebHostEnvironment;
        public ReportController(ApplicationDbContext context, IWebHostEnvironment iwebHostEnvironment, IHubContext<NotificationHub> hubContext)
        {
            _iwebHostEnvironment = iwebHostEnvironment;
            _hubContext = hubContext;
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            _context = context;
        }
        //public async Task<IActionResult> Upload_Photo(int requestId)
        //{
        //   var request = await _context.UserRequests.FirstOrDefaultAsync(e => e.Id == requestId);
        //    if (request == null)
        //        return NotFound();
        //    TempData["rId"] = requestId;
        //    return View();
        //}
        //[HttpPost]
        //public  IActionResult Upload_Photo(byte[] img)
        //{
        //    if (img == null)
        //        return NotFound();
        //    var requestId = Convert.ToInt32(TempData["rId"]);
        //    var request = _context.UserRequests.Include(e => e.Doctor).Include(e => e.Radiologist).FirstOrDefault(e => e.Id == requestId);
        //    TempData["eye_img"] = img;
        //    return RedirectToAction("GenerateReport");
        //}
        public async Task<IActionResult> GenerateReport()
        {
            var ReportId = Convert.ToInt32(TempData["rep_id"]);
            var report = await _context.UserReports.Include(e=>e.Request).Include(e=>e.Radiologist).Include(e=>e.Doctor).FirstOrDefaultAsync(e => e.Id == ReportId);
            if (report == null)
                return NotFound();
            return View(report);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateReportAsync(int reportId)
        {
            var report = _context.UserReports.Include(e => e.Request).Include(e => e.Radiologist).Include(e => e.Doctor).FirstOrDefault(e=>e.Id==reportId);
            if (report == null)
                return NotFound();
            string mimtype = "";
            int extension = 1;
            var path = $"{this._iwebHostEnvironment.WebRootPath}\\report\\Report1.rdlc";
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("ReportParameter1", "Date : " +report.Date.ToString());
            parameters.Add("ReportParameter8", "Doctor Name : " + report.Doctor.FirstName.ToString()+" "+report.Doctor.LastName.ToString());
            parameters.Add("ReportParameter9", "Radiologist Name : " + report.Radiologist.FirstName.ToString() + " " + report.Radiologist.LastName.ToString());
            parameters.Add("ReportParameter2", "Name : " + report.Request.FullName.ToString());
            parameters.Add("ReportParameter3", "Gender : " + report.Request.Gender.ToString());
            parameters.Add("ReportParameter4", "Age : " + report.Request.Age.ToString());
            parameters.Add("ReportParameter5", "Phone Number : " + report.Request.PhoneNumber.ToString());
            parameters.Add("ReportParameter6", "type disease in right eye : " + report.leftEyeDiseaseName.ToString());
            parameters.Add("ReportParameter10","type disease in right eye : " + report.rightEyeDiseaseName.ToString());
            parameters.Add("ReportParameter7", Convert.ToBase64String(report.LeftEye));
           parameters.Add("ReportParameter11", Convert.ToBase64String(report.RightEye));
            LocalReport localReport = new LocalReport(path);
            var result = localReport.Execute(RenderType.Pdf, extension, parameters, mimtype);
            report.ReportData = result.MainStream.ToArray();
            EntityEntry entityEntry = _context.Entry(report);
            entityEntry.State = EntityState.Modified;
            _context.SaveChanges();
            var notificationHub = _hubContext.Clients.All;
            await notificationHub.SendAsync("SendNewReport", "New Report Added");
            return RedirectToAction("Index", "Home");
            //   return File(result.MainStream, "application/pdf");
        }
        public async Task<IActionResult> GetReport(int ReportId)
        {
            var report =await _context.UserReports.Include(e => e.Doctor).Include(e => e.Radiologist).Include(e => e.Request).FirstOrDefaultAsync(e => e.Id == ReportId);
            if (report == null)
                return NotFound();
            TempData["reportID"] = ReportId;
            return View(report);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> GetReport()
        {
            int reportId = Convert.ToInt32(TempData["reportID"]);
            var report = await _context.UserReports.Include(e => e.Doctor).Include(e => e.Radiologist).Include(e => e.Request).FirstOrDefaultAsync(e => e.Id == reportId);
            if (report == null)
                return NotFound();
            var reportBytes = report.ReportData;
            return File(reportBytes, "application/pdf");
        }

    }
}
