using MassTransit;
using BookmarkAiApi.Contracts;
using BookmarkAiApi.Services;

namespace BookmarkAiApi.Consumers;


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