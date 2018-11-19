using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Tech_Forum.Models
{
    public class SubscriberLogin
    {
        [Display(Name ="Email ID")]
        [Required(AllowEmptyStrings =false,ErrorMessage ="Email ID required")]
        public string email { get; set; }

        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [Required(AllowEmptyStrings =false,ErrorMessage ="Password Required")]
        public string password { get; set; }
        
        [Display(Name ="Remember Me")]
        public bool RememberMe { get; set; }
    }
}