using System.Collections.Generic;
using System.Linq;
using BlogAPI.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace BlogAPI.Models
{
    public class User : IdentityUser, IUpdater
    {
        public List<Comment> LikedComments { get; set; }

        public List<Topic> Interests { get; set; }

        public UserResponse GetUserResponse() => new () {Email = Email, Id = Id, Interests = Interests.Select(x => x.Name), Username = UserName};
    }
}
