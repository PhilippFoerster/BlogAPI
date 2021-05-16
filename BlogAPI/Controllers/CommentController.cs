using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BlogAPI.Models;
using BlogAPI.Models.Request;
using BlogAPI.Models.Respond;
using BlogAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Type = BlogAPI.Models.Request.Type;

namespace BlogAPI.Controllers
{
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly CommentService commentService;

        private readonly ArticleService articleService;

        private readonly UserService userService;


        public CommentController(CommentService commentService, ArticleService articleService, UserService userService)
        {
            this.commentService = commentService;
            this.articleService = articleService;
            this.userService = userService;
        }

        [HttpPost]
        [Route("comments")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<CommentResponse>> PostComment(NewComment newComment)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Answer (ModelState.GetErrors(), Type.InvalidModel));
            try
            {
                var articleExists = await articleService.ArticleExists(newComment.ArticleId);
                if (!articleExists)
                    return NotFound(new Answer($"No article with id {newComment.ArticleId} was found"));
                var comment = await commentService.CreateComment(newComment, User.GetUserID());
                await commentService.InsertComment(comment);
                return CreatedAtAction("GetComment", new { id = comment.Id }, comment.GetCommentResponse());
            }
            catch (Exception e)
            {
                return StatusCode(500, new Answer("Error while creating comment"));
            }
        }

        [HttpGet]
        [Route("comments")]
        public async Task<ActionResult<List<CommentResponse>>> GetComments([FromQuery] int page = 1, [FromQuery] int pageSize = 9)
        {
            try
            {
                return Ok(await commentService.GetCommentResponses(User.GetUserID(), page, pageSize));
            }
            catch
            {
                return StatusCode(500, new Answer("Error while getting comments"));
            }
        }

        [HttpGet]
        [Route("articles/{articleId}/comments")]
        public async Task<ActionResult<List<CommentResponse>>> GetComments(int articleId, [FromQuery] int page = 1, [FromQuery] int pageSize = 9)
        {
            try
            {
                var articleExists = await articleService.ArticleExists(articleId);
                if (!articleExists)
                    return NotFound(new Answer($"No article with id {articleId} was found"));
                var comments = await commentService.GetCommentResponses(User.GetUserID(), page, pageSize, (int)articleId);
                return comments is not null ? Ok(comments) : NotFound(new Answer($"No comment related to article {articleId} was found"));
            }
            catch
            {
                return StatusCode(500, new Answer($"Error while getting comments of article {articleId}"));
            }
        }

        [HttpGet]
        [Route("comments/{id}")]
        public async Task<ActionResult<CommentResponse>> GetComment(int id)
        {
            try
            {
                var comment = await commentService.GetCommentResponse(id, User.GetUserID());
                return comment is not null ? Ok(comment) : NotFound(new Answer($"No comment with id {id} was found"));
            }
            catch
            {
                return StatusCode(500, new Answer($"Error while getting comment {id}"));
            }
        }

        [HttpDelete]
        [Route("comments/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "author, admin")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            try
            {
                var comment = await commentService.GetComment(id);
                if (comment is null)
                    return NotFound(new Answer($"No comment with id {id} was found"));
                await commentService.DeleteComment(comment);
                return NoContent();
            }
            catch
            {
                return StatusCode(500, new Answer("Error while deleting comment"));
            }
        }

        [HttpPost]
        [Route("comments/like")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<CommentResponse>> LikeComment(LikeComment like)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Answer(ModelState.GetErrors(), Type.InvalidModel));
            var comment = await commentService.GetComment(like.CommentId, q => q.IncludeLikes());
            if (comment is null)
                return NotFound(new Answer($"No comment with id {like.CommentId} was found"));
            var createdBy = await userService.GetUser(comment.CreatedById, q => q.AsNoTracking());
            try
            {
                var userId = User.GetUserID();
                comment = await commentService.LikeComment(comment, userId, like.Liked);
                if (comment is null)
                    return StatusCode(304, new Answer($"The comment was already {(like.Liked ? "liked" : "disliked")}", Type.Info));
                comment.CreatedBy = createdBy;
                return Ok(comment.GetCommentResponse(like.Liked));
            }
            catch
            {
                return StatusCode(500, new Answer($"Error while trying to {(like.Liked ? "like" : "dislike")} comment"));
            }
        }
    }
}
