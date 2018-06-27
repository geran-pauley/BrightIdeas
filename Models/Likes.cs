using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BrightIdeas.Models
{
    public class Likes
    {
        public int id { get; set; }

        public int users_id { get; set; }

        public int posts_id { get; set; }
    }
}