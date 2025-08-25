using BookmarkAI_API.Contracts;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace BookmarkAI_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EnqueueJobController : ControllerBase
    {
        private readonly IBus _bus;
        
        public EnqueueJobController(IBus bus)
        {
            _bus = bus;
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> EnqueueJob([FromBody] string data)
        {
            await _bus.Publish<IScrapperJob>(new
            {
                Url = data
            });
            return Ok($"Job with data '{data}' enqueued.");
        }
    }
}

