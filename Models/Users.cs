using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BrightIdeas.Models
{
    public class Users
    {
        //ID
        public int id { get; set; }

        //Name
        [Required( ErrorMessage = "First name is Required")]
        [MinLength(2, ErrorMessage = "First name must be at least 2 characters long")]
        public string name { get; set; }

        //Alias
        [Required(ErrorMessage = "Alias is required")]
        [MinLength(2, ErrorMessage = "Alias must be at least 2 characters long")]
        public string alias { get; set; }

        //EMail
        [Required(ErrorMessage = "An Email is required")]
        public string email { get; set; }

        //Password
        [Required(ErrorMessage = "A password is required")]
        [MinLength(8, ErrorMessage = "Your password must be 8 characters long")]
        public string password { get; set; }

        //Created at
        [Required]
        public DateTime created_at { get; set; }
    }
}