using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogAPI.Interfaces;

namespace BlogAPI.Models
{
    public class UserResponse : IUpdateable
    {
        public string Id { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public IEnumerable<string> Interests { get; set; } = new List<string>();

        public IList<string> Roles { get; set; } = new List<string>();

    }
}
