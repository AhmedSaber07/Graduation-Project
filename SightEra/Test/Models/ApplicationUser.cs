using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Test.Models
{
    public class ApplicationUser:IdentityUser
    {
        [RegularExpression("^[A-Za-z]+$", ErrorMessage = "Please Enter Valid Name")]
        [Required,MaxLength(100)]
        public string FirstName { get; set; }

        [RegularExpression("^[A-Za-z]+$", ErrorMessage = "Please Enter Valid Name")]
        [Required, MaxLength(100)]
        public string LastName { get; set; }

        [Required]
        public string UserType { get; set; }
        public byte[] ProfilePicture { get; set; }
    }
}
