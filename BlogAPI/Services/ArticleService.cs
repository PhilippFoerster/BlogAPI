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


        public async Task<List<Article>> GetArticles()
            => await blogContext.Articles.Include(x => x.CreatedBy).Select(x => new Article
            {
                CreatedAt = x.CreatedAt,
                Caption = x.Caption,
                CreatedBy = x.CreatedBy,
                Id = x.Id
            }).ToListAsync();

        public async Task<Article> GetArticle(int id, bool includeComments = false) => await blogContext.Articles.Include(x => x.CreatedBy).If(includeComments, x => x.Include(x => x.Comments)).FirstOrDefaultAsync(x => x.Id == id);

        public async Task<Article> CreateArticle(NewArticle newArticle, string user)
        {
            return new Article()
            {
                CreatedAt = DateTime.Now,
                CreatedBy = await userService.GetUser(user)
            }.UpdateFrom(newArticle);
        }

        public async Task DeleteArticle(Article article)
        {
            blogContext.Articles.Remove(article);
            await blogContext.SaveChangesAsync();
        }

        public async Task UpdateArticle(Article article)
        {
            blogContext.Articles.Update(article);
            await blogContext.SaveChangesAsync();
        }
    }
}
