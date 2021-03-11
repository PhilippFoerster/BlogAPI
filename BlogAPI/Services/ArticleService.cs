using BlogAPI.Database;
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

        public async Task<bool> InsertArticle(Article article)
        {
            blogContext.Articles.Add(article);
            foreach(var test in blogContext.ChangeTracker.Entries().ToArray())
            {
                if (test.Entity is User)
                    test.State = EntityState.Detached;
            }
            await blogContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> InsertComment(Comment comment)
        {
            blogContext.Comments.Add(comment);
            await blogContext.SaveChangesAsync();
            return true;
        }

        public async Task<List<Article>> GetArticles()
        {
            return await blogContext.Articles.Include(x => x.CreatedBy).ToListAsync();
        }

        public Article CreateArticle(NewArticle newArticle, string user)
        {
            return new Article
            {
                Caption = newArticle.Caption,
                Text = newArticle.Text,
                Image = newArticle.Image,
                CreatedAt = DateTime.Now,
                CreatedBy = userService.GetUserByNameOrMail(user)
            };
        }

        public Comment CreateComment(NewComment newComment, string user)
        {
            return new Comment
            {
                ArticleId = newComment.ArticleId,
                Text = newComment.Text,
                CreatedBy = userService.GetUserByNameOrMail(user)
            };
        }
    }
}
