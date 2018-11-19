using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Tech_Forum.Models
{
    [MetadataType(typeof(PostMetaData))]
    public partial class Post_Table
    {

    }

    public class PostMetaData
    {
        [Display(Name = "Domain")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Domain required")]
        public string domain { get; set; }

        [Display(Name = "Technology")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Technology required")]
        public string technology { get; set; }

        [Display(Name = "Title")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Title required")]
        public string title { get; set; }

        [Display(Name = "Tags")]
        public string tags { get; set; }

        [Display(Name = "Content")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Content required")]
        [MinLength(200, ErrorMessage = "Content should be minimum of 200 characters")]
        public string content_ { get; set; }
    }
}