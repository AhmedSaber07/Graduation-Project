using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Test.ViewModel
{
    public class DisplayPhotos
    {
        [Required(ErrorMessage ="Please Enter Photo")]
        [Display(Name ="RightPhoto")]
        public byte[] right { get; set; }
        [Required(ErrorMessage = "Please Enter Photo")]
        [Display(Name = "LeftPhoto")]
        public byte[] Left { get; set; }
    }
}
