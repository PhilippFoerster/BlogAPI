using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace BlogAPI.Models
{
    public class User : IdentityUser
    {
        public List<Comment> LikedComments { get; set; }

        public List<Topic> Interests { get; set; }

        public UserResponse GetUserResponse() => new() { Email = Email, Id = Id, Username = UserName };
    }
}
