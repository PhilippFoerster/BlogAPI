using System;
using BlogAPI.Models;
using BlogAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BlogAPI.Models.Request;
using BlogAPI.Models.Respond;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Type = BlogAPI.Models.Request.Type;

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
        [Route("articles")]
        [Auth("admin","author")]
        public async Task<ActionResult<ArticleResponse>> PostArticle(NewArticle newArticle)
        {
            var test = User.Claims;
            if (!ModelState.IsValid)
                return BadRequest(new Answer(ModelState.GetErrors(), Type.InvalidModel));
            try
            {
                newArticle.Topics = newArticle.Topics.Distinct().ToList();
                var article = articleService.CreateArticle(newArticle, User.GetUserID());
                await articleService.InsertArticle(article);
                return CreatedAtAction("GetArticle", new { id = article.Id }, article.GetArticleResponse());
            }
            catch (Exception e) {
                return StatusCode(500, new Answer("Error while creating article"));
            }
        }

        [HttpGet]
        [Route("articles")]
        public async Task<ActionResult<ArticlesResponse>> GetArticles([FromQuery] List<string> topics, [FromQuery] int page = 1, [FromQuery] int pageSize = 9)
        {
            try
            {
                return Ok(await articleService.GetArticleResponses(topics, page, pageSize));
            }
            catch
            {
                return StatusCode(500, new Answer("Error while getting articles"));
            }
        }

        [HttpGet]
        [Route("articles/top")]
        public async Task<ActionResult<List<ArticleResponse>>> GetTopArticles()
        {
            try
            {
                return Ok(await articleService.GetTopArticleResponses());
            }
            catch
            {
                return StatusCode(500, new Answer("Error while getting articles"));
            }
        }

        [HttpGet]
        [Route("articles/{id}")]
        public async Task<ActionResult<ArticleResponse>> GetArticle(int id)
        {
            try
            {
                var article = await articleService.GetArticleResponse(id);
                return article is not null ? Ok(article) : NotFound(new Answer($"No article with id {id} was found"));
            }
            catch
            {
                return StatusCode(500, new Answer($"Error while getting Article {id}"));
            }
        }


        [HttpPut]
        [Route("articles")]
        [Auth("admin", "author")]
        public async Task<ActionResult<ArticleResponse>> ModifyArticle(UpdateArticle updateArticle)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Answer(ModelState.GetErrors(), Type.InvalidModel));
            try
            {
                var article = await articleService.GetArticle(updateArticle.Id);
                if (article is null)
                    return NotFound(new Answer($"No article with id {updateArticle.Id} was found"));
                if (!User.IsInRole("admin") && article.CreatedBy.Id != User.GetUserID())
                    return Unauthorized();
                article.Caption = updateArticle.Caption;
                article.Image = updateArticle.Image ?? "";
                article.Text = updateArticle.Text;
                article.Topics = updateArticle.Topics?.Select(x => new Topic{Name = x}).ToList() ?? new List<Topic>();
                await articleService.UpdateArticle(article);
                article.CreatedBy = await articleService.GetAuthor(article);
                return CreatedAtAction("GetArticle", new { id = article.Id }, article.GetArticleResponse());
            }
            catch
            {
                return StatusCode(500, new Answer($"Error while modifying article {updateArticle.Id}"));
            }
        }


        [HttpDelete]
        [Route("articles/{id}")]
        [Auth("admin")]
        public async Task<IActionResult> DeleteArticle(int id)
        {
            try
            {
                var article = await articleService.GetArticle((int)id);
                if (article is null)
                    return NotFound(new Answer($"No article with id {id} was found"));
                await articleService.DeleteArticle(article);
                return NoContent();
            }
            catch
            {
                return StatusCode(500, new Answer("Error while deleting article"));
            }
        }
    }
}
