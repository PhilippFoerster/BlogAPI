using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlogAPI.Models
{
    public class NewArticle
    {
        [Required]
        public string Image { get; set; }

        [Required]
        public string Caption { get; set; }

        [Required]
        [StringLength(Settings.ArticleTextLength, ErrorMessage = "An article's text can't be longer than {1}")]
        public string Text { get; set; }

        [Required]
        public List<string> Topics { get; set; }
    }
}
