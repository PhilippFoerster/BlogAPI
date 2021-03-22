using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using BlogAPI.Interfaces;

namespace BlogAPI.Models
{
    public class NewComment : IUpdater
    {
        public string Text { get; set; }

        public int? ArticleId { get; set; }
    }
}
