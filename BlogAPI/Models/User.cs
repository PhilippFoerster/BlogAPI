using System.Collections.Generic;
using BlogAPI.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace BlogAPI.Models
{
    public class User : IdentityUser, IUpdater
    {
        public List<Comment> LikedComments { get; set; }

        public List<Topic> Interests { get; set; }
    }
}
