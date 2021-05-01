using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BlogAPI.Models
{
    public class LikeComment
    {
        [Required]
        public int CommentId { get; set; }

        [Required]
        public bool Liked { get; set; }
    }
}
