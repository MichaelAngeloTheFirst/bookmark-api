namespace BookmarkAI_API.Services;

using System.Runtime.CompilerServices;
using BookmarkAI_API.Modules;


public class JobScrapperService
{
    private readonly Scrapper _scrapper;

    public JobScrapperService(Scrapper scrapper)
    {
        _scrapper = scrapper;
    }

    public async Task GetMarkdown(string url)
    {
        Console.WriteLine($"Fetching and converting URL: {url}");
        var markdown = await _scrapper.GetMarkdown(url);
        Console.WriteLine(markdown);
        await Task.Delay(500);
        return;
    }
}