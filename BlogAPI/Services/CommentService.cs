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
            return new Comment
            {
                CreatedBy = await userService.GetUserByNameOrMail(user),
                CreatedAt = DateTime.Now
            }.UpdateFrom(newComment);
        }

        public async Task<List<CommentResponse>> GetCommentResponses() => await blogContext.Comments.Include(x => x.CreatedBy).IncludeLikes().SelectResponse().ToListAsync();
        public async Task<List<CommentResponse>> GetCommentResponses(int articleId) => await blogContext.Comments.Where(x => x.ArticleId == articleId).Include(x => x.CreatedBy).IncludeLikes().SelectResponse().ToListAsync();

        public async Task<Comment> GetCommentWithLikes(int id) => await blogContext.Comments.IncludeLikes().FirstOrDefaultAsync(x => x.Id == id);
        public async Task<Comment> GetComment(int id) => await blogContext.Comments.FirstOrDefaultAsync(x => x.Id == id);
        public async Task<CommentResponse> GetCommentResponse(int id) => await blogContext.Comments.Include(x => x.CreatedBy).IncludeLikes().SelectResponse().FirstOrDefaultAsync(x => x.Id == id);

        public async Task DeleteComment(Comment comment)
        {
            blogContext.Comments.Remove(comment);
            await blogContext.SaveChangesAsync();
        }

        public async Task<Comment> LikeComment(Comment comment, string userId, bool liked)
        {
            var user = new User { Id = userId };
            var hasLiked = await blogContext.Comments.Where(x => x.Id == comment.Id && x.LikedBy.Any(x => x.Id == user.Id)).IncludeLikes().FirstOrDefaultAsync() is not null;
            Action action = null;
            if (!hasLiked && liked)
            {
                comment.LikedBy = new List<User>();
                action = () => comment.LikedBy.Add(user);
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
            blogContext.Attach(comment);
            action();
            await blogContext.SaveChangesAsync();
            comment.LikedBy = null;
            comment.CreatedBy.LikedComments = null;
            return comment;
        }
    }
}
