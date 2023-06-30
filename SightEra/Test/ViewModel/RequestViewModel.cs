using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Test.ViewModel
{
    public class RequestViewModel
    {
        public int Id { get; set; }
        [RegularExpression(@"^[A-Za-z\s-']+$ ", ErrorMessage = "Please Enter Valid Name")]
        [Required]
        [Display(Name = "Full Name")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Full Name must be between 6 and 50 chars")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "Gender Is Required")]
        public string Gender { get; set; }
        [Phone]
        [Required(ErrorMessage = "Phone Number Is Required")]
        [Display(Name = "Phone Number")]
        [RegularExpression("^01[0-2|5]{1}[0-9]{8}$", ErrorMessage = "Please Enter Valid Number")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Age Is Required")]
        public int Age { get; set; }
        public bool IsShow { get; set; }
    }
}
