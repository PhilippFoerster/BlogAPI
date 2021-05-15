using System.Linq;
using BlogAPI.Models;
using BlogAPI.Models.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BlogAPI
{
    public class BlogContext : IdentityDbContext
    {
        public DbSet<Article> Articles { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public new DbSet<User> Users { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }


        public BlogContext(DbContextOptions<BlogContext> options) : base(options) { }


        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseLoggerFactory(LoggerFactory.Create(x => x.AddDebug()));
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<User>()
                .HasMany(x => x.Comments)
                .WithOne(x => x.CreatedBy)
                .HasForeignKey(x => x.CreatedById)
                .OnDelete(DeleteBehavior.NoAction);
            builder.Entity<User>()
                .HasMany(x => x.Articles)
                .WithOne(x => x.CreatedBy)
                .HasForeignKey(x => x.CreatedById)
                .OnDelete(DeleteBehavior.NoAction);
            builder.Entity<User>()
                .HasMany(x => x.LikedComments)
                .WithMany(x => x.LikedBy);
            builder.Entity<User>()
                .HasMany(x => x.RefreshTokens)
                .WithOne(x => x.User)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Article>()
                .HasMany(x => x.Comments)
                .WithOne(x => x.Article)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Article>()
                .HasMany(x => x.Topics)
                .WithMany(x => x.Articles);
        }
    }
}
