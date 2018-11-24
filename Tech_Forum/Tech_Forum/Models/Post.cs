using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Tech_Forum.Models
{
    [MetadataType(typeof(PostMetaData))]
    public class Post : Content
    {
        public string title { get; set; }
        public string domain { get; set; }
        public string technology { get; set; }
        public string tags { get; set; }
        public Nullable<double> rating { get; set; }

    }
}