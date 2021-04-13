using BlogAPI.Models;
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


        public BlogContext(DbContextOptions<BlogContext> options) : base(options) { }


        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseLoggerFactory(LoggerFactory.Create(x => x.AddDebug()));
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Comment>()
                .HasOne(x => x.CreatedBy)
                .WithMany()
                .HasForeignKey(x => x.CreatedById)
                .OnDelete(DeleteBehavior.NoAction);
            builder.Entity<Article>()
                .HasOne(x => x.CreatedBy)
                .WithMany()
                .HasForeignKey(x => x.CreatedById)
                .OnDelete(DeleteBehavior.NoAction);
            builder.Entity<User>()
                .HasMany(x => x.LikedComments)
                .WithMany(x => x.LikedBy);
            builder.Entity<Article>()
                .HasMany(x => x.Topics)
                .WithMany(x => x.Articles);
        }
    }
}
