using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using BlogAPI.Models.Respond;
using Newtonsoft.Json;

namespace BlogAPI.Models
{
    public class Article
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "varchar(max)")]
        public string Image { get; set; }

        [MaxLength(100)]
        [Required]
        public string Caption { get; set; }

        [Required]
        [MaxLength(Settings.ArticleTextMaxLength)]
        public string Text { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public string CreatedById { get; set; }
        
        public User CreatedBy { get; set; }

        public List<Comment> Comments { get; set; }

        public List<Topic> Topics { get; set; }


        public ArticleResponse GetArticleResponse() => new()
        {
            Topics = Topics?.Select(x => x.Name).ToList() ?? new List<string>(),
            Caption = Caption,
            CreatedAt = CreatedAt,
            CreatedBy = CreatedBy.GetUserResponse(),
            Id = Id,
            Text = Text,
            Image = Image
        };
    }
}
