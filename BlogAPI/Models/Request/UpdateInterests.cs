using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BlogAPI.Models
{
    public class UpdateInterests
    {
        [Required]
        public List<string> Interests { get; set; }
    }
}
