using BookmarkAI_API.Contracts;
using MassTransit;
using Microsoft.AspNetCore.Mvc;


namespace BookmarkAI_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EnqueueJobController : ControllerBase
    {
        private readonly IPublishEndpoint _publishEndpoint;
        
        public EnqueueJobController(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> EnqueueJob([FromBody] string data)
        {
            await _publishEndpoint.Publish<IScrapperJob>(new
            {
                Url = data
            });
            return Ok($"Job with data '{data}' enqueued.");
        }
    }
}

