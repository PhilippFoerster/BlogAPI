using BlogAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using BlogAPI.Models.Respond;
using Microsoft.AspNetCore.Identity;

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
            blogContext.Attach(article.CreatedBy);
            var existing = await blogContext.Topics.Where(x => article.Topics.Contains(x)).AsNoTracking().ToListAsync(); //get existing topics in db
            article.Topics.Intersect(existing, new TopicComparer()).ToList().ForEach(x => blogContext.Topics.Attach(x)); //attach the not tracked topic from the request, not the one loaded from db
            await blogContext.Articles.AddAsync(article);
            await blogContext.SaveChangesAsync();
        }

        public async Task<bool> ArticleExists(int id) => await blogContext.Articles.AnyAsync(x => x.Id == id);

        public async Task<List<ArticleResponse>> GetTopArticleResponses()
            => await blogContext.Articles
                .OrderByDescending(x => x.Comments.Count / (1 + EF.Functions.DateDiffDay(DateTime.Now, x.CreatedAt) / 30)) //Algorithm to get "trending" articles
                .ThenByDescending(x => x.CreatedAt)
                .SelectResponse()
                .Take(4)
                .ToListAsync();

        public async Task<ArticlesResponse> GetArticleResponses(List<string> topics, int page, int pageSize)
        {
            var articles = blogContext.Articles.Include(x => x.CreatedBy)
                .Include(x => x.Topics)
                .If(topics.Count > 0, q => q.Where(x => x.Topics.Any(x => topics.Contains(x.Name))))
                .SelectResponse()
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            var res = new ArticlesResponse {Articles = await articles.ToListAsync(), TotalCount = await blogContext.Articles.If(topics.Count > 0, q => q.Where(x => x.Topics.Any(x => topics.Contains(x.Name)))).CountAsync()};

            res.Articles.ForEach(article =>
            {
                if (article.Text.Length > 200)
                    article.Text = article.Text.Substring(0, 197) + "...";
            });
            return res;
        }

        public async Task<Article> GetArticle(int id) => await blogContext.Articles.FirstOrDefaultAsync(x => x.Id == id);

        public async Task<User> GetAuthor(Article article) 
            => await blogContext.Users.Where(x => x.Id == blogContext.Articles.Find(article.Id).CreatedById).FirstOrDefaultAsync();
        
        public async Task<ArticleResponse> GetArticleResponse(int id)
            => await blogContext.Articles.Include(x => x.CreatedBy)
                .Include(x => x.Topics)
                .SelectResponse()
                .FirstOrDefaultAsync(x => x.Id == id);

        public Article CreateArticle(NewArticle newArticle, string userId)
        {
            return new ()
            {
                CreatedAt = DateTime.Now,
                CreatedBy = new User{Id = userId},
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
            var existing = await blogContext.Topics.Where(x => article.Topics.Contains(x)).AsNoTracking().ToListAsync();
            article.Topics.Intersect(existing, new TopicComparer()).ToList().ForEach(x => blogContext.Topics.Attach(x));
            blogContext.Articles.Update(article);
            await blogContext.SaveChangesAsync();
        }
    }
}
