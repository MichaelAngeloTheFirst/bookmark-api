using HtmlAgilityPack;

namespace BookmarkAiApi.Modules;


public class HtmlCleaner
{
    static string CleanHtml(string htmlContent)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(htmlContent);
        
        var nodesToRemove = doc.DocumentNode.SelectNodes("//script|//style|//noscript|//iframe|//link|//meta");
        if (nodesToRemove != null)
        {
            foreach (var node in nodesToRemove)
            {
                node.Remove();
            }
        }

        return doc.DocumentNode.OuterHtml;
    }
}