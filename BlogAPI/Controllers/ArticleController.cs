﻿using System;
using BlogAPI.Models;
using BlogAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogAPI.Models.Respond;
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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin, author")]
        public async Task<ActionResult<ArticleResponse>> PostArticle(NewArticle newArticle)
        {
            if (newArticle.HasNullProperty())
                return BadRequest("Missing properties!");
            try
            {
                var article = articleService.CreateArticle(newArticle, User.GetUserID());
                await articleService.InsertArticle(article);
                return CreatedAtAction("GetArticle", new { id = article.Id }, article.GetArticleResponse());
            }
            catch (Exception e) {
                return StatusCode(500, "Error while creating article");
            }
        }

        [HttpGet]
        [Route("articles")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin, author")]
        public async Task<ActionResult<List<ArticleResponse>>> GetArticles([FromQuery] List<string> topics, [FromQuery] int page = 1, [FromQuery] int pageSize = 9)
        {
            try
            {
                return Ok(await articleService.GetArticleResponses(topics, page, pageSize));
            }
            catch
            {
                return StatusCode(500, "Error while getting articles");
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
                return StatusCode(500, "Error while getting articles");
            }
        }

        [HttpGet]
        [Route("articles/{id}")]
        public async Task<ActionResult<ArticleResponse>> GetArticle(int? id)
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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin, author")]
        public async Task<ActionResult<ArticleResponse>> ModifyArticle(UpdateArticle updateArticle)
        {
            try
            {
                var article = await articleService.GetArticle(updateArticle.Id);
                if (article is null)
                    return NotFound($"No article with id {updateArticle.Id} was found");
                article.Caption = updateArticle.Caption;
                article.Image = updateArticle.Image;
                article.Text = updateArticle.Text;
                await articleService.UpdateArticle(article);
                article.CreatedBy = await articleService.GetAuthor(article);
                return CreatedAtAction("GetArticle", new { id = article.Id }, article.GetArticleResponse());
            }
            catch
            {
                return StatusCode(500, $"Error while modifying article {updateArticle.Id}");
            }
        }


        [HttpDelete]
        [Route("articles/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
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
