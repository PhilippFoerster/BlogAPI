using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BlogAPI.Models
{
    public class UpdateRoles
    {
        [Required]
        public string Role { get; set; }
    }
}
