using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BrightIdeas.Models
{
    public class Posts
    {
        public int id { get; set; }

        public int usersID { get; set; }

        [Required]
        public string message { get; set; }
        
        public int likes { get; set; }
    }
}