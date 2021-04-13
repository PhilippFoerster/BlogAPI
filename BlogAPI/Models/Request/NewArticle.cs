using System.Collections.Generic;

namespace BlogAPI.Models
{
    public class NewArticle
    {
        public string Image { get; set; }

        public string Caption { get; set; }

        public string Text { get; set; }

        public List<string> Topics { get; set; }
    }
}
