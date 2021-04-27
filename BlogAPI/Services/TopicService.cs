using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogAPI.Models;
using BlogAPI.Models.Respond;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.Services
{
    public class TopicService
    {
        private readonly BlogContext blogContext;

        public TopicService(BlogContext blogContext)
        {
            this.blogContext = blogContext;
        }

        public async Task<TopicResponse> GetTopics(int? limit)
        {
            var topics = await blogContext.Topics
                .OrderByDescending(x => x.Articles.Count)
                .If(limit is not null, x => x.Take((int) limit))
                .ToListAsync();
            return new TopicResponse {Topics = topics.Select(x => x.Name).ToList()};
        }
    }
}
