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
        UserContext userContext;

        public UserService(UserContext userContext)
        {
            this.userContext = userContext;
        }

        public bool IsAuthorized(HttpContext context, Role[] roles)
        {
            string header = context.Request.Headers["Authorization"];
            if (header != null && header.StartsWith("Basic"))
            {
                string encoded = header.Substring(6);
                string userPass = Encoding.UTF8.GetString(Convert.FromBase64String(encoded));
                var split = userPass.Split(":");
                if (split.Length != 1)
                    return false;
                string username = split[0];
                string password = split[1];

                var user = userContext.Users.FirstOrDefault(x => x.Username == username);
                if (user is null || !roles.Contains(user.Role))
                    return false;
                var hasher = new PasswordHasher<string>();
                var verification = hasher.VerifyHashedPassword(username, user.Password, password);
                return verification == PasswordVerificationResult.Success;
            }
            return false;
        }

        public async Task<bool> CreateUser(User user)
        {
            var hasher = new PasswordHasher<string>();
            user.Password = hasher.HashPassword(user.Username, user.Password);
            userContext.Users.Add(user);
            try
            {
                await userContext.SaveChangesAsync();
            }
            catch(Exception e)
            {
                return false;
            }
            return true;
        }
    }
}
