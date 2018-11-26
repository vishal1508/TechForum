using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tech_Forum.Models
{
    public class PostParentModel
    {
        public Article article { get; set; }
        public Comment comment { get; set; }
        public Subscriber_Table subscriberTable { get; set; }
    }
}