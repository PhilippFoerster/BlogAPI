namespace BlogAPI.Models
{
    public class UpdateArticle
    {
        public int Id { get; set; }
        public string Image { get; set; }

        public string Caption { get; set; }

        public string Text { get; set; }
    }
}
