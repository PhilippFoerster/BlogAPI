using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BlogAPI.Models
{
    public class Topic
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public List<Article> Articles { get; set; }
    }
}
