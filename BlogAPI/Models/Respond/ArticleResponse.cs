using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogAPI.Models.Respond
{
    public class ArticleResponse
    { public int Id { get; set; }
        public string Image { get; set; }

        public string Caption { get; set; }

        public string Text { get; set; }

        public DateTime CreatedAt { get; set; }

        public UserResponse CreatedBy { get; set; }

        public IEnumerable<CommentResponse> Comments { get; set; }

        public IEnumerable<string> Topics { get; set; }
    }
}
