using HtmlAgilityPack;
using Markdig;

namespace BookmarkAI_API.Modules;

public class HtmlConverter
{
    public  string ConvertHtmlToMarkdown(string htmlContent)
    {
        var markdown = Markdown.ToPlainText(htmlContent);
        return markdown;
    }
}