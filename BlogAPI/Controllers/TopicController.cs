using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogAPI.Models.Respond;
using BlogAPI.Services;

namespace BlogAPI.Controllers
{
    public class TopicController : ControllerBase
    {
        private readonly TopicService topicService;

        public TopicController(TopicService topicService)
        {
            this.topicService = topicService;
        }

        [HttpGet]
        [Route("topics")]
        public async Task<ActionResult<TopicResponse>> GetTopics([FromQuery] int? limit)
        {
            try
            {
                return Ok(await topicService.GetTopics(limit));
            }
            catch
            {
                return StatusCode(500, "Error while getting topics");
            }
        }
    }
}
