using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BlogAPI.Attributes;
using BlogAPI.Models;
using BlogAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlogAPI.Controllers
{
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly CommentService commentService;
        private readonly UserService userService;

        public CommentController(CommentService commentService, UserService userService)
        {
            this.commentService = commentService;
            this.userService = userService;
        }

        [HttpPost]
        [Route("comment")]
        [Auth(Role.User, Role.Author)]
        public async Task<ActionResult<User>> PostComment(NewComment newComment)
        {
            if (newComment.HasNullProperty())
                return BadRequest("Missing properties!");
            try
            {
                var comment = await commentService.CreateComment(newComment, Request.GetUser());
                await commentService.InsertComment(comment);
                return CreatedAtAction("GetComment", new { id = comment.Id }, comment);
            }
            catch
            {
                return StatusCode(500, "Error while creating comment");
            }
        }

        [HttpGet]
        [Route("comments")]
        public async Task<ActionResult<List<Comment>>> GetComments()
        {
            try
            {
                return await commentService.GetComments();
            }
            catch
            {
                return StatusCode(500, $"Error while getting comments");
            }
        }

        [HttpGet]
        [Route("comments/{articleId}")]
        public async Task<ActionResult<List<Comment>>> GetComments(int? articleId)
        {
            if (articleId is null)
                return BadRequest("Missing id");
            try
            {
                var comments = await commentService.GetComments((int)articleId);
                return comments is not null ? comments : NotFound($"No comment related to article {articleId} was found");
            }
            catch
            {
                return StatusCode(500, $"Error while getting comments of article {articleId}");
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
                var comment = await commentService.GetComment((int)id);
                return comment is not null ? comment : NotFound($"No comment with id {id} was found");
            }
            catch
            {
                return StatusCode(500, $"Error while getting comment {id}");
            }
        }

        [HttpDelete]
        [Route("comment/{id}")]
        [Auth(Role.Admin)]
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
        [Route("comment/like")]
        [Auth(Role.User, Role.Author)]
        public async Task<ActionResult<Comment>> LikeComment(LikeComment like)
        {
            if (like.HasNullProperty())
                return BadRequest("Missing properties!");
            var comment = await commentService.GetComment((int) like.CommentId);
            if(comment is null)
                return NotFound($"No comment with id {like.CommentId} was found");
            string text = (bool)like.Liked ? "like" : "dislike";
            try
            {
                var userId = User.GetUserID();
                comment = await commentService.LikeComment(comment, userId, (bool)like.Liked);
                return comment is not null ? comment : StatusCode(304);
            }
            catch
            {
                return StatusCode(500, $"Error while trying to {text} comment");
            }
        }
    }
}
