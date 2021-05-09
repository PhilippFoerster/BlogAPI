using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlogAPI.Models
{
    public class NewArticle
    {
        [Required]
        public string Image { get; set; }

        [Required]
        [MinLength(Settings.ArticleCaptionMinLength, ErrorMessage = "An article's text length be at least {1}")]
        public string Caption { get; set; }

        [Required]
        [StringLength(Settings.ArticleTextMaxLength, MinimumLength = Settings.ArticleTextMinLength, ErrorMessage = "An article's text must be at least {2} long and can't be longer than {1}")]
        public string Text { get; set; }

        public List<string> Topics { get; set; }
    }
}
