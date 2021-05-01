using System.ComponentModel.DataAnnotations;

namespace BlogAPI.Models
{
    public class NewComment
    {
        [Required]
        [StringLength(Settings.CommentTextLength, ErrorMessage = "A comment's text can't be longer than {1}")]
        public string Text { get; set; }

        [Required]
        public int ArticleId { get; set; }
    }
}
