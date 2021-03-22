using BlogAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogAPI.Models.Respond;

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
            article.Topics.Where(x => blogContext.Topics.Any(t => x.Name == t.Name)).ToList().ForEach(x => blogContext.Topics.Attach(x));
            blogContext.Attach(article.CreatedBy);
            await blogContext.Articles.AddAsync(article);
            await blogContext.SaveChangesAsync();
        }


        public async Task<List<ArticleResponse>> GetArticleResponses(List<string> topics)
            => await blogContext.Articles.Include(x => x.CreatedBy).Include(x => x.Topics)
                .If(topics.Count > 0, q => q.Where(x => x.Topics.Any(x => topics.Contains(x.Name))))
                .SelectResponse()
                .ToListAsync();

        public async Task<Article> GetArticle(int id) => await blogContext.Articles.FirstOrDefaultAsync(x => x.Id == id);

        public async Task<ArticleResponse> GetArticleResponse(int id, bool includeComments = false)
            => await blogContext.Articles.Include(x => x.CreatedBy)
                .Include(x => x.Topics)
                .If(includeComments, x => x.Include(x => x.Comments))
                .SelectResponse()
                .FirstOrDefaultAsync(x => x.Id == id);

        public async Task<Article> CreateArticle(NewArticle newArticle, string userId)
        {
            return new ()
            {
                CreatedAt = DateTime.Now,
                CreatedBy = new User { Id = userId },
                Topics = newArticle.Topics.Select(x => new Topic { Name = x }).ToList(),
                Caption = newArticle.Caption,
                Text = newArticle.Text,
                Image = newArticle.Image
            };
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
