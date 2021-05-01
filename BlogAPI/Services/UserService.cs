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

        private readonly UserManager<IdentityUser> userManager;

        public UserService(BlogContext blogContext, IConfiguration configuration, UserManager<IdentityUser> userManager)
        {
            this.blogContext = blogContext;
            this.configuration = configuration;
            this.userManager = userManager;
        }
        
        public async Task<UserResponse> GetUserResponse(string id) => await blogContext.Users.Include(x => x.Interests).SelectResponse().FirstOrDefaultAsync(x => x.Id == id);

        public async Task<User> GetUser(string id, Func<IQueryable<User>, IQueryable<User>> func) => await blogContext.Users.Apply(func).FirstOrDefaultAsync(x => x.Id == id);
        public async Task<User> GetUser(string id) => await blogContext.Users.FirstOrDefaultAsync(x => x.Id == id);

        public async Task UpdateTopics(User user, List<Topic> oldTopics, List<Topic> newTopics)
        {
            var comparer = new TopicComparer();
            user.Interests = oldTopics.Intersect(newTopics, comparer).ToList();
            var existing = await blogContext.Topics.Where(x => newTopics.Contains(x)).ToListAsync();
            var notExisting = newTopics.Except(existing, comparer).ToList();
            var added = existing.Concat(notExisting).Except(oldTopics, comparer);

            user.Interests.AddRange(added);
            await blogContext.SaveChangesAsync();
        }

        public async Task<List<Topic>> GetInterests(string userId) => await blogContext.Topics.Where(x => x.InterestedUser.Contains(new User { Id = userId})).ToListAsync();
        public async Task<List<string>> GetRoles(string userId) => (await userManager.GetRolesAsync(new User { Id = userId })).ToList();

        public async Task<string> GenerateJwt(IdentityUser user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var claims = new List<Claim>
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
