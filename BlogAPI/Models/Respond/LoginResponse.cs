using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogAPI.Models.Respond
{
    public class LoginResponse
    {
        public string Jwt { get; set; }
        public string RefreshToken { get; set; }
    }
}
