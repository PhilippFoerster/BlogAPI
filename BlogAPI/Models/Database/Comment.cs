using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BlogAPI.Models.Respond;
using Newtonsoft.Json;

namespace BlogAPI.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(Settings.CommentTextMaxLength)]

        public string Text { get; set; }
        public DateTime CreatedAt { get; set; }

        [Required]
        public int ArticleId { get; set; }

        [JsonIgnore]
        public string CreatedById { get; set; }

        public Article Article { get; set; }

        public User CreatedBy { get; set; }

        public List<User> LikedBy { get; set; }

        [NotMapped] 
        public int Likes { get; set; }


        public CommentResponse GetCommentResponse(bool liked = false) => new() {ArticleId = ArticleId, Text = Text, CreatedAt = CreatedAt, CreatedBy = CreatedBy?.GetUserResponse(), Id = Id, Likes = Likes, Liked = liked};
    }
}
