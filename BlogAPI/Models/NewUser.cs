using BlogAPI.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogAPI.Models
{
    public class NewUser
    {
        public string Mail { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

    }
}
