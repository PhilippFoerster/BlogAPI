using System.Collections.Generic;
using BlogAPI.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using BlogAPI.Interfaces;

namespace BlogAPI.Models
{
    public class User : IUpdateable
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(50)]
        [Required]
        [JsonIgnore]
        public string Mail { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        [MaxLength(100)]
        [JsonIgnore]
        public string Password { get; set; }

        public List<Comment> LikedComments { get; set; }

        public List<Topic> Interests { get; set; }

        [Required]
        public Role Role { get; set; }
    }
}
