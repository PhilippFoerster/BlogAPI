using System.Collections.Generic;
using BlogAPI.Models.Database;
using Microsoft.AspNetCore.Identity;

namespace BlogAPI.Models
{
    public class User : IdentityUser
    {
        public List<Comment> LikedComments { get; set; }

        public List<Topic> Interests { get; set; }

        public List<RefreshToken> RefreshTokens { get; set; }

        public List<Article> Articles { get; set; }

        public List<Comment> Comments { get; set; }

        public UserResponse GetUserResponse() => new() { Email = Email, Id = Id, Username = UserName };
    }
}
