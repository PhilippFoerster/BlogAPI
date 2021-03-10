using BlogAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.Database
{
    public class UserContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public UserContext() { }

        public UserContext(DbContextOptions<UserContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlServer("Server=DESKTOP-BT4H8CA;Database=Blog;Trusted_Connection=True");
    }
}