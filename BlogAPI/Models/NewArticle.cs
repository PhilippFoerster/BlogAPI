using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogAPI.Interfaces;

namespace BlogAPI.Models
{
    public class NewArticle : IUpdater
    {
        public string Image { get; set; }

        public string Caption { get; set; }

        public string Text { get; set; }

        public List<Topic> Topics { get; set; }
    }
}
