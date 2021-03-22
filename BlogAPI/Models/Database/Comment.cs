using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using BlogAPI.Interfaces;
using BlogAPI.Models.Respond;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace BlogAPI.Models
{
    public class Comment : IUpdateable
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Text { get; set; }
        public DateTime CreatedAt { get; set; }

        [Required]
        public int ArticleId { get; set; }

        [Required]
        [JsonIgnore]
        public string CreatedById { get; set; }

        public Article Article { get; set; }

        public User CreatedBy { get; set; }

        public List<User> LikedBy { get; set; }

        [NotMapped] 
        public int Likes { get; set; }


        public CommentResponse GetCommentResponse() => new() {ArticleId = ArticleId, Text = Text, CreatedAt = CreatedAt, CreatedBy = CreatedBy.GetUserResponse(), Id = Id, Likes = Likes};
    }
}
