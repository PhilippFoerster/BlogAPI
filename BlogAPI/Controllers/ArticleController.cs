using System;
using BlogAPI.Models;
using BlogAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, Author")]
        public async Task<IActionResult> PostArticle(NewArticle newArticle)
        {
            if (newArticle.HasNullProperty())
                return BadRequest("Missing properties!");
            try
            {
                var article = await articleService.CreateArticle(newArticle, User.GetUserID());
                await articleService.InsertArticle(article);
                return CreatedAtAction("GetArticle", new { id = article.Id }, article.GetArticleResponse());
            }
            catch(Exception e)
            {
                return StatusCode(500, "Error while creating article");
            }
        }

        [HttpGet]
        [Route("articles")]
        public async Task<IActionResult> GetArticles([FromQuery] List<string> topics)
        {
            try
            {
                return Ok(await articleService.GetArticleResponses(topics));
            }
            catch
            {
                return StatusCode(500, "Error while getting articles");
            }
        }

        [HttpGet]
        [Route("articles/{id}")]
        public async Task<IActionResult> GetArticle(int? id)
        {
            if (id is null)
                return BadRequest("Missing id");
            try
            {
                var article = await articleService.GetArticleResponse((int)id);
                return article is not null ? Ok(article) : NotFound($"No article with id {id} was found");
            }
            catch
            {
                return StatusCode(500, $"Error while getting Article {id}");
            }
        }


        [HttpPut]
        [Route("articles")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, Author")]
        public async Task<IActionResult> ModifyArticle(UpdateArticle updateArticle)
        {
            try
            {
                var article = await articleService.GetArticle(updateArticle.Id);
                if (article is null)
                    return NotFound($"No article with id {updateArticle.Id} was found");
                article.UpdateFrom(updateArticle);
                await articleService.UpdateArticle(article);
                return CreatedAtAction("GetArticle", new { id = article.Id }, article.GetArticleResponse());
            }
            catch
            {
                return StatusCode(500, $"Error while modifying article {updateArticle.Id}");
            }
        }


        [HttpDelete]
        [Route("articles/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
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
