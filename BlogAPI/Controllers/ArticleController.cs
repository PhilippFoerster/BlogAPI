using BlogAPI.Models;
using BlogAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlogAPI.Attributes;

namespace BlogAPI.Controllers
{
    [ApiController]
    public class ArticleController : ControllerBase
    {
        private readonly ArticleService articleService;
        public ArticleController(ArticleService articleService)
        {
            this.articleService = articleService;
        }

        [HttpPost]
        [Route("comment/new")]
        [Auth(Role.User, Role.Author)]
        public async Task<ActionResult<User>> PostComment(NewComment newComment)
        {
            try
            {
                var comment = await articleService.CreateComment(newComment, Request.GetUser());
                await articleService.InsertComment(comment);
                return CreatedAtAction("GetComment", new { id = comment.Id }, comment);
            }
            catch
            {
                return StatusCode(500, "Error while creating comment");
            }
        }

        [HttpGet]
        [Route("comment/{id}")]
        public async Task<ActionResult<Comment>> GetComment(int? id)
        {
            if (id is null)
                return BadRequest("Missing id");
            try
            {
                var comment = await articleService.GetComment((int)id);
                if (comment is not null)
                    return comment;
                return NotFound($"No comment with id {id} was found");
            }
            catch
            {
                return StatusCode(500, $"Error while getting comment {id}");
            }
        }


        [HttpPost]
        [Route("article/new")]
        [Auth(Role.Author)]
        public async Task<ActionResult<User>> PostArticle(NewArticle newArticle)
        {
            try
            {
                var article = await articleService.CreateArticle(newArticle, Request.GetUser());
                await articleService.InsertArticle(article);
                return CreatedAtAction("GetArticle", new { id = article.Id }, article);
            }
            catch
            {
                return StatusCode(500, "Error while creating article");
            }
        }

        [HttpGet]
        [Route("articles")]
        public async Task<ActionResult<List<Article>>> GetArticles()
        {
            try
            {
                return await articleService.GetArticles();
            }
            catch
            {
                return StatusCode(500, "Error while getting articles");
            }
        }

        [HttpGet]
        [Route("article/{id}")]
        public async Task<ActionResult<Article>> GetArticle(int? id)
        {
            if (id is null)
                return BadRequest("Missing id");
            try
            {
                var article = await articleService.GetArticle((int)id);
                if (article is not null)
                    return article;
                return NotFound($"No article with id {id} was found");
            }
            catch
            {
                return StatusCode(500, $"Error while getting Article {id}");
            }
        }

        [HttpDelete]
        [Route("comment/delete/{id}")]
        [Auth(Role.Admin)]
        public async Task<IActionResult> DeleteComment(int? id)
        {
            if (id is null)
                return BadRequest("Missing id");
            try
            {
                var comment = await articleService.GetComment((int)id);
                if (comment is null)
                    return NotFound($"No comment with id {id} was found");
                await articleService.DeleteComment(comment);
                return NoContent();
            }
            catch
            {
                return StatusCode(500, "Error while deleting comment");
            }
        }
    }
}
