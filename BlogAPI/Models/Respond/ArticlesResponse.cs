using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogAPI.Models.Respond
{
    public class ArticlesResponse
    {
        public int TotalCount { get; set; }

        public List<ArticleResponse> Articles { get; set; }
    }
}
