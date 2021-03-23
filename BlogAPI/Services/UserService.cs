using BlogAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BlogAPI.Services
{
    public class UserService
    {
        private readonly BlogContext blogContext;

        private readonly IConfiguration configuration;

        private UserManager<IdentityUser> userManager;

        public UserService(BlogContext blogContext, IConfiguration configuration, UserManager<IdentityUser> userManager)
        {
            this.blogContext = blogContext;
            this.configuration = configuration;
            this.userManager = userManager;
        }
        
        public async Task<UserResponse> GetUserResponse(string id) => await blogContext.Users.Include(x => x.Interests).SelectResponse().FirstOrDefaultAsync(x => x.Id == id);

        public async Task<User> GetUser(string id) =>
            await blogContext.Users.Include(x => x.Interests).FirstOrDefaultAsync(x => x.Id == id);

        public async Task UpdateTopics(List<Topic> topics)
        {
            topics.Where(x => blogContext.Topics.Any(t => x.Name == t.Name)).ToList().ForEach(x => blogContext.Topics.Attach(x));
            await blogContext.SaveChangesAsync();
        }

        public async Task<string> GenerateJwt(IdentityUser user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            List<Claim> claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.UserName),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new (ClaimTypes.Role, string.Join(",", await userManager.GetRolesAsync(user))),
                new("id", user.Id)
            };


            var token = new JwtSecurityToken(configuration["Jwt:Issuer"],
                configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddHours(12),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
