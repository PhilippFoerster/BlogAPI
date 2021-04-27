using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BlogAPI.Models;
using BlogAPI.Models.Respond;
using BlogAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogAPI.Controllers
{
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly CommentService commentService;

        public CommentController(CommentService commentService)
        {
            this.commentService = commentService;
        }

        [HttpPost]
        [Route("comments")]
        [Authorize]
        public async Task<ActionResult<CommentResponse>> PostComment(NewComment newComment)
        {
            if (newComment.HasNullProperty())
                return BadRequest("Missing properties!");
            try
            {
                var comment = await commentService.CreateComment(newComment, User.GetUserID());
                await commentService.InsertComment(comment);
                return CreatedAtAction("GetComment", new { id = comment.Id }, comment.GetCommentResponse());
            }
            catch
            {
                return StatusCode(500, "Error while creating comment");
            }
        }

        [HttpGet]
        [Route("comments")]
        public async Task<ActionResult<List<CommentResponse>>> GetComments([FromQuery] int page = 1, [FromQuery] int pageSize = 9)
        {
            try
            {
                return Ok(await commentService.GetCommentResponses(page, pageSize));
            }
            catch
            {
                return StatusCode(500, $"Error while getting comments");
            }
        }

        [HttpGet]
        [Route("articles/{articleId}/comments")]
        public async Task<ActionResult<List<CommentResponse>>> GetComments(int? articleId, [FromQuery] int page = 1, [FromQuery] int pageSize = 9)
        {
            if (articleId is null)
                return BadRequest("Missing id");
            try
            {
                var comments = await commentService.GetCommentResponses(page, pageSize,(int)articleId);
                return comments is not null ? Ok(comments) : NotFound($"No comment related to article {articleId} was found");
            }
            catch
            {
                return StatusCode(500, $"Error while getting comments of article {articleId}");
            }
        }

        [HttpGet]
        [Route("comments/{id}")]
        public async Task<ActionResult<CommentResponse>> GetComment(int? id)
        {
            if (id is null)
                return BadRequest("Missing id");
            try
            {
                var comment = await commentService.GetCommentResponse((int)id);
                return comment is not null ? Ok(comment) : NotFound($"No comment with id {id} was found");
            }
            catch
            {
                return StatusCode(500, $"Error while getting comment {id}");
            }
        }

        [HttpDelete]
        [Route("comments/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "author, admin")]
        public async Task<IActionResult> DeleteComment(int? id)
        {
            if (id is null)
                return BadRequest("Missing id");
            try
            {
                var comment = await commentService.GetComment((int)id);
                if (comment is null)
                    return NotFound($"No comment with id {id} was found");
                await commentService.DeleteComment(comment);
                return NoContent();
            }
            catch
            {
                return StatusCode(500, "Error while deleting comment");
            }
        }

        [HttpPost]
        [Route("comments/like")]
        [Authorize]
        public async Task<ActionResult<CommentResponse>> LikeComment(LikeComment like)
        {
            if (like.HasNullProperty())
                return BadRequest("Missing properties!");
            var comment = await commentService.GetCommentWithLikes((int) like.CommentId);
            if(comment is null)
                return NotFound($"No comment with id {like.CommentId} was found");
            string text = (bool)like.Liked ? "like" : "dislike";
            try
            {
                var userId = User.GetUserID();
                comment = await commentService.LikeComment(comment, userId, (bool)like.Liked);
                return comment is not null ? Ok(comment.GetCommentResponse()) : StatusCode(304);
            }
            catch
            {
                return StatusCode(500, $"Error while trying to {text} comment");
            }
        }
    }
}
