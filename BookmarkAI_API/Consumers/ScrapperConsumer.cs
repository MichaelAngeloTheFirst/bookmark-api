using BookmarkAI_API.Contracts;
using MassTransit;
using BookmarkAI_API.Services;

namespace BookmarkAI_API.Consumers;

public class ScrapperConsumer : IConsumer<IScrapperJob>
{
    private readonly JobScrapperService _jobScrapperService;
    
    public ScrapperConsumer(JobScrapperService jobScrapperService)
    {
        _jobScrapperService = jobScrapperService;
    }
    
    public async Task Consume(ConsumeContext<IScrapperJob> context)
    {
        await _jobScrapperService.GetMarkdown(context.Message.Url);
    }
    
    
}