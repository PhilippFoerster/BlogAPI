using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.Services
{
    public class CommentService
    {
        private readonly BlogContext blogContext;
        private readonly UserService userService;

        public CommentService(BlogContext blogContext, UserService userService)
        {
            this.blogContext = blogContext;
            this.userService = userService;
        }

        public async Task InsertComment(Comment comment)
        {
            await blogContext.Comments.AddAsync(comment);
            await blogContext.SaveChangesAsync();
        }

        public async Task<Comment> CreateComment(NewComment newComment, string user)
        {
            return new Comment
            {
                CreatedBy = await userService.GetUser(user),
                CreatedAt = DateTime.Now
            }.UpdateFrom(newComment);
        }

        public async Task<List<Comment>> GetComments() => await blogContext.Comments.Include(x => x.CreatedBy).ToListAsync();
        public async Task<List<Comment>> GetComments(int articleId) => await blogContext.Comments.Where(x => x.ArticleId == articleId).Include(x => x.CreatedBy).ToListAsync();
        
        public async Task<Comment> GetComment(int id) => await blogContext.Comments.FirstOrDefaultAsync(x => x.Id == id);
        public async Task DeleteComment(Comment comment)
        {
            blogContext.Comments.Remove(comment);
            await blogContext.SaveChangesAsync();
        }
    }
}
