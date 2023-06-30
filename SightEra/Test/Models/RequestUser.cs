using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Test.Models
{
    public class RequestUser
    {
        public int Id { get; set; }
        [RegularExpression("^[a-zA-Z]{3,}(?: [a-zA-Z]{3,}){0,2}$", ErrorMessage ="Please Enter Valid Name")]
        [Required]
        [Display(Name = "Full Name")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Full Name must be between 6 and 50 chars")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "Gender Is Required")]
        public string Gender { get; set; }
        [Phone]
        [Required(ErrorMessage = "Phone Number Is Required")]
        [Display(Name = "Phone Number")]
        [RegularExpression("^01[0-2|5]{1}[0-9]{8}$",ErrorMessage ="Please Enter Valid Number")]
        public string PhoneNumber { get; set; }
       [RegularExpression("^(1[0-9]|[2-9][0-9])$" , ErrorMessage ="Enter Valid Age")]
        [Required(ErrorMessage = "Age Is Required")]
        //10-99
        public int Age { get; set; }

        public bool IsShow { get; set; }
        public DateTime RequestDate { get; set; }
        //Relations
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }
        public Radiologist Radiologist { get; set; }
        public int RadiologistId { get; set; }
    }
}
