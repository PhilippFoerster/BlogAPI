using BlogAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BlogAPI
{
    public class BlogContext : DbContext
    {
        public DbSet<Article> Articles { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<User> Users { get; set; }


        public BlogContext(DbContextOptions<BlogContext> options) : base(options) { }

        //private string connectionString = "Server=tcp:gruppe5blog.database.windows.net,1433;Initial Catalog=Blog;Persist Security Info=False;User ID=gruppe5admin;Password=bouncer1aquaria1SILICATE-quetzal!baseball;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        private string connectionString = "Server=DESKTOP-BT4H8CA;Database=Blog;Trusted_Connection=True";

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(connectionString);
            options.UseLoggerFactory(LoggerFactory.Create(x => x.AddDebug()));
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>(x => x.HasAlternateKey(user => user.Mail));
            builder.Entity<User>(x => x.HasAlternateKey(user => user.Username));
        }
    }
}
