using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BlogAPI.Models.Request
{
    public class Refresh
    {
        [Required]
        public string Jwt { get; set; }

        [Required]
        public string RefreshToken { get; set; }
    }
}
