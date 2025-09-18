using BookmarkAiApi.Contracts;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace BookmarkAiApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EnqueueJobController(IBus bus) : ControllerBase
    {
        [HttpPost]
        [Route("")]
        public async Task<IActionResult> EnqueueJob([FromBody] string data)
        {
            await bus.Publish<IScrapperJob>(new
            {
                Url = data
            });
            return Ok($"Job with data '{data}' enqueued.");
        }
    }
}

