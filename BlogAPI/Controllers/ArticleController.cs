using BlogAPI.Models;
using BlogAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlogAPI.Controllers
{
    [ApiController]
    public class ArticleController : ControllerBase
    {
        ArticleService articleService;
        public ArticleController(ArticleService articleService)
        {
            this.articleService = articleService;
        }

        [HttpPost]
        [Route("comment/new")]
        public async Task<ActionResult<User>> PostComment(NewComment newComment)
        {
            var comment = articleService.CreateComment(newComment, Request.GetUser());
            if (await articleService.InsertComment(comment))
                return StatusCode(201, new Response(false, "created successfully"));
            return StatusCode(400, new Response(true, "error while creating new user"));
        }

        [HttpPost]
        [Route("article/new")]
        public async Task<ActionResult<User>> PostArticle(NewArticle newArticle)
        {
            var article = articleService.CreateArticle(newArticle, Request.GetUser());
            if (await articleService.InsertArticle(article))
                return StatusCode(201, new Response(false, "created successfully"));
            return StatusCode(400, new Response(true, "error while creating new user"));
        }

        [HttpGet]
        [Route("articles")]
        public async Task<ActionResult<List<Article>>> GetArticles()
        {
            return await articleService.GetArticles();
        }
    }
}
