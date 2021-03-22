using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogAPI.Models.Respond
{
    public class CommentResponse
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public DateTime CreatedAt { get; set; }

        public int ArticleId { get; set; }

        public UserResponse CreatedBy { get; set; }

        public int Likes { get; set; }
    }
}
