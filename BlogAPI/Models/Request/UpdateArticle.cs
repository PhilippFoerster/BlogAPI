using System.ComponentModel.DataAnnotations;

namespace BlogAPI.Models
{
    public class UpdateArticle
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Image { get; set; }

        [Required]
        public string Caption { get; set; }

        [Required]
        public string Text { get; set; }
    }
}
