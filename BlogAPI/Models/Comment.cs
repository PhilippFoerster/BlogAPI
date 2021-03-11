using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BlogAPI.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Text { get; set; }

        public int ArticleId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public User CreatedBy { get; set; }
    }
}
