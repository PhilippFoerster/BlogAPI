using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace BlogAPI.Models.Database
{
    public class RefreshToken
    {
        [Key] 
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Token { get; set; }

        public string Jwt { get; set; }

        public DateTime ExpirationTime { get; set; }

        public User User { get; set; }
    }
}
