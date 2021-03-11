using BlogAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.Database
{
    public class BlogContext : DbContext
    {
        public DbSet<Article> Articles { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<User> Users { get; set; }

        public BlogContext(DbContextOptions<BlogContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlServer("Server=DESKTOP-BT4H8CA;Database=Blog;Trusted_Connection=True");

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>(x => x.HasAlternateKey(user => user.Mail));
            builder.Entity<User>(x => x.HasAlternateKey(user => user.Username));
        }
    }
}
