using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Tech_Forum.Models
{
    public class Comment : Content
    {
        public List<Comment> comments = new List<Comment>();

        static public int maxlength = 100;

    }
}