using BlogAPI.Attributes;
using BlogAPI.Database;
using BlogAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogAPI.Services
{
    public class UserService
    {
        private readonly BlogContext blogContext;

        public UserService(BlogContext blogContext)
        {
            this.blogContext = blogContext;
        }

        public bool IsAuthorized(HttpContext context, Role[] roles)
        {
            string header = context.Request.Headers["Authorization"];
            if (header != null && header.StartsWith("Basic"))
            {
                string encoded = header[6..];
                string userPass = Encoding.UTF8.GetString(Convert.FromBase64String(encoded));
                var split = userPass.Split(":");
                if (split.Length != 2)
                    return false;
                string username = split[0];
                string password = split[1];

                var user = blogContext.Users.FirstOrDefault(x => x.Username == username || x.Mail == username);
                if (user is null || !roles.Contains(user.Role) && user.Role != Role.Admin)
                    return false;
                var hasher = new PasswordHasher<string>();
                var verification = hasher.VerifyHashedPassword(user.Username, user.Password, password);
                return verification == PasswordVerificationResult.Success;
            }
            return false;
        }

        public async Task<bool> CreateUser(User user)
        {
            var hasher = new PasswordHasher<string>();
            user.Password = hasher.HashPassword(user.Username, user.Password);
            blogContext.Users.Add(user);
            try
            {
                await blogContext.SaveChangesAsync();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public User GetUserByNameOrMail(string user) => blogContext.Users.FirstOrDefault(x => x.Username == user || x.Mail == user);
        

    }
}
