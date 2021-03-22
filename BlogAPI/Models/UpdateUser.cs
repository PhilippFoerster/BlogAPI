using System.Collections.Generic;
using BlogAPI.Attributes;
using BlogAPI.Interfaces;

namespace BlogAPI.Models
{
    public class UpdateUser : IUpdater
    {
        public string Id { get; set; }

        public List<string> Roles { get; set; }

        public List<string> Interests { get; set; }
    }
}
