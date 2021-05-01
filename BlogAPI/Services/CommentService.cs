using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using BlogAPI.Models;
using BlogAPI.Models.Respond;
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
            return new ()
            {
                CreatedBy = await userService.GetUser(user),
                CreatedAt = DateTime.Now,
                ArticleId = newComment.ArticleId,
                Text = newComment.Text
            };
        }

        public async Task<List<CommentResponse>> GetCommentResponses(string userId, int page, int pageSize, int? articleId = null) 
            => await blogContext.Comments
                .If(articleId is not null, q => q.Where(x => x.ArticleId == articleId))
                .Skip((page - 1) * pageSize).Take(pageSize)
                .Include(x => x.CreatedBy)
                .SelectResponse(userId).ToListAsync();

        public async Task<Comment> GetComment(int id, Func<IQueryable<Comment>, IQueryable<Comment>> func) => await blogContext.Comments.Apply(func).FirstOrDefaultAsync(x => x.Id == id);
        public async Task<Comment> GetComment(int id) => await blogContext.Comments.FirstOrDefaultAsync(x => x.Id == id);
        public async Task<CommentResponse> GetCommentResponse(int id, string userId) => await blogContext.Comments.Include(x => x.CreatedBy).SelectResponse(userId).FirstOrDefaultAsync(x => x.Id == id);

        public async Task DeleteComment(Comment comment)
        {
            blogContext.Comments.Remove(comment);
            await blogContext.SaveChangesAsync();
        }

        public async Task<Comment> LikeComment(Comment comment, string userId, bool liked)
        {
            var user = new User { Id = userId };
            var hasLiked = await blogContext.Comments.AnyAsync(x => x.Id == comment.Id && x.LikedBy.Any(x => x.Id == user.Id));
            Action action = null;
            if (!hasLiked && liked)
            {
                action = () => comment.LikedBy.Add(user);
                comment.LikedBy = new List<User>();
                comment.Likes++;
            }
            else if (hasLiked && !liked)
            {
                action = () => comment.LikedBy.Remove(user);
                comment.LikedBy = new List<User> { user };
                comment.Likes--;
            }
            if (action is null)
                return null;
            blogContext.Attach(user);
            blogContext.Attach(comment);
            action();
            await blogContext.SaveChangesAsync();
            return comment;
        }
    }
}
