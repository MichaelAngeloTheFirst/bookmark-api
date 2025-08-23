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
       var markdown = await _jobScrapperService.GetMarkdown(context.Message.Url);
       Console.WriteLine($"Markdown for URL '{context.Message.Url}': {markdown}");
       await context.Publish<IMarkdownCleaned>(new
       {
           markdown = markdown,
       });
    }
    
    
}