using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogAPI.Interfaces;

namespace BlogAPI.Models
{
    public class NewUser : IUpdater
    {
        public string Mail { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

    }
}
