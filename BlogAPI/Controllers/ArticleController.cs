using System;
using BlogAPI.Models;
using BlogAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
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
        [Route("article")]
        [Auth(Role.Author)]
        public async Task<ActionResult<User>> PostArticle(NewArticle newArticle)
        {
            if (newArticle.HasNullProperty())
                return BadRequest("Missing properties!");
            try
            {
                var article = await articleService.CreateArticle(newArticle, Request.GetUser());
                await articleService.InsertArticle(article);
                article.Topics.ForEach(x => x.Articles = null);
                return CreatedAtAction("GetArticle", new { id = article.Id }, article);
            }
            catch(Exception e)
            {
                return StatusCode(500, "Error while creating article");
            }
        }

        [HttpGet]
        [Route("articles")]
        public async Task<ActionResult<List<Article>>> GetArticles([FromQuery] List<string> topics)
        {
            try
            {
                return await articleService.GetArticles(topics);
            }
            catch
            {
                return StatusCode(500, "Error while getting articles");
            }
        }

        [HttpGet]
        [Route("article/{id}")]
        public async Task<ActionResult<Article>> GetArticle(int? id, bool includeComments = false)
        {
            if (id is null)
                return BadRequest("Missing id");
            try
            {
                var article = await articleService.GetArticle((int)id, includeComments);
                return article is not null ? article : NotFound($"No article with id {id} was found");
            }
            catch
            {
                return StatusCode(500, $"Error while getting Article {id}");
            }
        }


        [HttpPut]
        [Route("article")]
        [Auth(Role.Admin)]
        public async Task<ActionResult<User>> ModifyUser(UpdateArticle updateArticle)
        {
            try
            {
                var article = await articleService.GetArticle(updateArticle.Id);
                if (article is null)
                    return NotFound($"No article with id {updateArticle.Id} was found");
                article.UpdateFrom(updateArticle);
                await articleService.UpdateArticle(article);
                return CreatedAtAction("GetArticle", new { id = article.Id }, article);
            }
            catch
            {
                return StatusCode(500, $"Error while modifying article {updateArticle.Id}");
            }
        }


        [HttpDelete]
        [Route("article/{id}")]
        [Auth(Role.Admin)]
        public async Task<IActionResult> DeleteArticle(int? id)
        {
            if (id is null)
                return BadRequest("Missing id");
            try
            {
                var article = await articleService.GetArticle((int)id);
                if (article is null)
                    return NotFound($"No article with id {id} was found");
                await articleService.DeleteArticle(article);
                return NoContent();
            }
            catch
            {
                return StatusCode(500, "Error while deleting article");
            }
        }
    }
}
