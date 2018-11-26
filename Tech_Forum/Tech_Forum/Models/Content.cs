using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Tech_Forum.Models
{
    
    public class Content
    {
        public string postid { get; set; }
        public string content_ { get; set; }
        public System.DateTime date { get; set; }
        public string userid { get; set; }
    }
}
