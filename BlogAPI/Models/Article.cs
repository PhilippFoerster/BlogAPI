using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using BlogAPI.Interfaces;

namespace BlogAPI.Models
{
    public class Article : IUpdateable
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "varchar(max)")]
        public string Image { get; set; }

        [MaxLength(100)]
        [Required]
        public string Caption { get; set; }

        [Required]
        public string Text { get; set; }

        public List<Comment> Comments { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public User CreatedBy { get; set; }
    }
}
