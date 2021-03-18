using BlogAPI.Attributes;
using BlogAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
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
            if (header == null || !header.StartsWith("Basic"))
                return false;
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

        public async Task<User> CreateUser(NewUser newUser)
        {
            var hasher = new PasswordHasher<string>();
            newUser.Password = hasher.HashPassword(newUser.Username, newUser.Password);
            var user = new User().UpdateFrom(newUser);
            await blogContext.Users.AddAsync(user);
            await blogContext.SaveChangesAsync();
            return user;
        }

        public async Task<User> GetUser(string user) => await blogContext.Users.FirstOrDefaultAsync(x => x.Username == user || x.Mail == user);
        public async Task<User> GetUser(int id) => await blogContext.Users.FirstOrDefaultAsync(x => x.Id == id);

        public async Task UpdateUser(User user)
        {
            blogContext.Users.Update(user);
            await blogContext.SaveChangesAsync();
        }

        public async Task DeleteUser(User user)
        {
            blogContext.Users.Remove(user);
            await blogContext.SaveChangesAsync();
        }

        public async Task<bool> IsUserUnique(NewUser user) => !await blogContext.Users.AnyAsync(x => x.Mail == user.Mail || x.Username == user.Username);
    }
}
