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
        public DbSet<Topic> Topics { get; set; }


        public BlogContext(DbContextOptions<BlogContext> options) : base(options) { }


        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseLoggerFactory(LoggerFactory.Create(x => x.AddDebug()));
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
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
            builder.Entity<User>(x => x.HasAlternateKey(user => user.Mail));
            builder.Entity<User>(x => x.HasAlternateKey(user => user.Username));
            builder.Entity<User>()
                .HasMany(x => x.LikedComments)
                .WithMany(x => x.LikedBy);
            builder.Entity<Article>()
                .HasMany(x => x.Topics)
                .WithMany(x => x.Articles);
        }
    }
}
