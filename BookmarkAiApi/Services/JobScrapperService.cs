using Markdig;
using System.Runtime.CompilerServices;
using BookmarkAiApi.Modules;

namespace BookmarkAiApi.Services;




public class JobScrapperService(Scrapper scrapper, HtmlConverter htmlConverter)
{
    public async Task<string> GetMarkdown(string url)
    {
        Console.WriteLine($"Fetching and converting URL: {url}");
        var html = await scrapper.GetHtml(url);
        var markdown = htmlConverter.ConvertHtmlToMarkdown(html);
        await Task.Delay(500);
        return markdown;
    }
}