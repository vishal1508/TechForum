using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Tech_Forum.Models
{
    [MetadataType(typeof(SubscriberMetadata))]
    public partial class Subscriber_Table
    {
        public string confirmpassword { get; set; }
    }

    public class SubscriberMetadata
    {
        [Display(Name ="Username")]
        [Required(AllowEmptyStrings =false,ErrorMessage ="Username required")]
        public string userid { get; set; }

        [Display(Name = "Full Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Full Name required")]
        public string name { get; set; }

        [Display(Name = "Email ID")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email ID required")]
        [DataType(DataType.EmailAddress)]
        public string email { get; set; }

        [Display(Name = "Password")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password required")]
        [DataType(DataType.Password)]
        [MinLength(6,ErrorMessage ="Password should be minimum of 6 characters")]
        public string password { get; set; }

        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("password",ErrorMessage ="Passwords do not match")]
        public string confirmpassword { get; set; }

        [Display(Name = "Mobile Number")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Mobile number required")]
        [DataType(DataType.PhoneNumber)]
        [MinLength(10, ErrorMessage = "Invalid mobile number")]
        [MaxLength(10, ErrorMessage = "Invalid mobile number")]
        public string mobile { get; set; }

        [Display(Name = "Describe Yourself")]
        public string description { get; set; }

    }
}