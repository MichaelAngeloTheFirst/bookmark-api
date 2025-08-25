using Markdig;
using System.Runtime.CompilerServices;
using BookmarkAI_API.Modules;

namespace BookmarkAI_API.Services;




public class JobScrapperService
{
    private readonly Scrapper _scrapper;
    private readonly HtmlConverter _htmlConverter;

    public JobScrapperService(Scrapper scrapper, HtmlConverter htmlConverter)
    {
        _scrapper = scrapper;
        _htmlConverter = htmlConverter;
    }

    public async Task<string> GetMarkdown(string url)
    {
        Console.WriteLine($"Fetching and converting URL: {url}");
        var html = await _scrapper.GetHtml(url);
        var markdown = _htmlConverter.ConvertHtmlToMarkdown(html);
        await Task.Delay(500);
        return markdown;
    }
}