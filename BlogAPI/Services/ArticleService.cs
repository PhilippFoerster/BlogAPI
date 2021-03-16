using BlogAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogAPI.Services
{
    public class ArticleService
    {
        private readonly BlogContext blogContext;
        private readonly UserService userService;

        public ArticleService(BlogContext blogContext, UserService userService)
        {
            this.blogContext = blogContext;
            this.userService = userService;
        }

        public async Task InsertArticle(Article article)
        {
            await blogContext.Articles.AddAsync(article);
            await blogContext.SaveChangesAsync();
        }

        public async Task InsertComment(Comment comment)
        {
            await blogContext.Comments.AddAsync(comment);
            await blogContext.SaveChangesAsync();
        }

        public async Task<List<Article>> GetArticles() => await blogContext.Articles.Include(x => x.CreatedBy).ToListAsync();

        public async Task<Article> GetArticle(int id) => await blogContext.Articles.Include(x => x.CreatedBy).Include(x => x.Comments).FirstOrDefaultAsync(x => x.Id == id);

        public async Task<Article> CreateArticle(NewArticle newArticle, string user)
        {
            return new()
            {
                Caption = newArticle.Caption,
                Text = newArticle.Text,
                Image = newArticle.Image,
                CreatedAt = DateTime.Now,
                CreatedBy = await userService.GetUserByNameOrMail(user)
            };
        }

        public async Task<Comment> CreateComment(NewComment newComment, string user)
        {
            return new()
            {
                ArticleId = newComment.ArticleId,
                Text = newComment.Text,
                CreatedBy = await userService.GetUserByNameOrMail(user),
                CreatedAt = DateTime.Now
            };
        }

        public async Task<Comment> GetComment(int id) => await blogContext.Comments.FirstOrDefaultAsync(x => x.Id == id);
        public async Task DeleteComment(Comment comment)
        {
            blogContext.Comments.Remove(comment);
            await blogContext.SaveChangesAsync();
        }
    }
}
