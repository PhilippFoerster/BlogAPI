using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BlogAPI.Models
{
    public class Login
    {
        [Required]
        public string User { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
