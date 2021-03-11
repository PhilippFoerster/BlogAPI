using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BlogAPI.Models
{
    public class NewComment
    {
        public string Text { get; set; }

        public int ArticleId { get; set; }
    }
}
