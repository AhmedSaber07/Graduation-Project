using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Test.Models
{
    public class ReportUser
    {
        public int Id { get; set; }
        
        [Required]
        [Display(Name ="Type of disease in RightEye")]
        public string rightEyeDiseaseName { get; set; }
        
        [Required]
        [Display(Name = "Type of disease in leftEye")]
        public string leftEyeDiseaseName { get; set; }
       
        [Required]
        [Display(Name = "Right Eye")]
        public byte[] RightEye { get; set; }
        
        [Required]
        [Display(Name = "Left Eye")]
        public byte[] LeftEye { get; set; }
        public string fileName1 { get; set; }
        public string fileName2 { get; set; }
        public DateTime Date { get; set; }
        public byte[] ReportData { get; set; }
        //Relations
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }
        public Radiologist Radiologist { get; set; }
        public int RadiologistId { get; set; }
       public int RequestId { get; set; }
       public RequestUser Request { get; set; }
    }
}
