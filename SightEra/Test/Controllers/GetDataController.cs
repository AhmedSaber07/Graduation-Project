using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Test.Data;
using Newtonsoft.Json;
namespace Test.Controllers
{
    public class GetDataController : Controller
    {
            private readonly IHttpClientFactory _httpClientFactory;
            private readonly ApplicationDbContext _context;

            public GetDataController(IHttpClientFactory httpClientFactory, ApplicationDbContext context)
            {
                _httpClientFactory = httpClientFactory;
                _context = context;
            }

        public async Task<IActionResult> CallPredictApi()
            {
            TempData["ShowVideo"] = true;
            var ReportId = Convert.ToInt32(TempData["rep_id"]);
                var report = _context.UserReports.Include(e=>e.Request).FirstOrDefault(e => e.Id == ReportId);
                if (report == null)
                    return NotFound();
                var httpClient = _httpClientFactory.CreateClient();
                var apiUrl = "http://127.0.0.1:5000/predictApi";
            //var imagePath = report.ImagePath;
            //var fileContent = new StreamContent(System.IO.File.OpenRead(imagePath));
                var image1Content = new ByteArrayContent(report.RightEye);
                 var image2Content = new ByteArrayContent(report.LeftEye);
                var formData = new MultipartFormDataContent();
                formData.Add(image1Content, "fileup", report.fileName1);
                formData.Add(image2Content, "fileup2", report.fileName2);
                var response = await httpClient.PostAsync(apiUrl, formData);

                // Check if the request was successful
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                string[] values = jsonResponse.Split(':');
                string v1 = values[1].Trim();
                string v2 = values[2].Trim();
                if (!String.IsNullOrEmpty(v1))
                {
                    if (v1.Contains("glaucoma"))
                    {
                        v1 = "glaucoma";
                    }
                    else if (v1.Contains("cataract"))
                    {
                        v1 = "cataract";
                    }
                    else if (v1.Contains("diabetic_retinopathy"))
                    {
                        v1 = "diabetic_retinopathy";
                    }
                    else if (v1.Contains("normal"))
                    {
                        v1 = "normal";
                    }
                }
                if (!String.IsNullOrEmpty(v2))
                {
                    if (v2.Contains("glaucoma"))
                    {
                        v2 = "glaucoma";
                    }
                    else if (v2.Contains("cataract"))
                    {
                        v2 = "cataract";
                    }
                    else if (v2.Contains("diabetic_retinopathy"))
                    {
                        v2 = "diabetic_retinopathy";
                    }
                    else if (v2.Contains("normal"))
                    {
                        v2 = "normal";
                    }
                }
                // "prediction ": "cataract , normal"
                //var result = jsonResponse.Substring(jsonResponse.IndexOf(':'));
                //var rightEye = result.Split(',');
                //var leftEye = jsonResponse.Substring(result.IndexOf(','));
                // var key = data[0].Trim();
                //var value = data[1].Trim();
                //value = value.Substring(0,value.Length-1);
                // ViewData["data"] = jsonResponse;
                //ViewData["key"] = key;
                report.rightEyeDiseaseName = v1;
                    report.leftEyeDiseaseName = v2;
                report.Request.IsShow = true;
                report.Date = DateTime.Now;
                EntityEntry entityEntry = _context.Entry(report);
                entityEntry.State = EntityState.Modified;
                _context.SaveChanges();
                ViewData["value1"] = v1;
                ViewData["value2"] = v2;
                TempData["ShowVideo"] = false;
                // Pass the JSON data to the view
                return RedirectToAction("GenerateReport", "Report");
                }
                else
                {
                    return View("ErrorView");
                }
            }
        }
    }