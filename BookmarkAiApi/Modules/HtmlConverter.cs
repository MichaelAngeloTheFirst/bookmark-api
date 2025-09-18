using HtmlAgilityPack;
using Markdig;

namespace BookmarkAiApi.Modules;

public class HtmlConverter
{
    public  string ConvertHtmlToMarkdown(string htmlContent)
    {
        var markdown = Markdown.ToPlainText(htmlContent);
        return markdown;
    }
}