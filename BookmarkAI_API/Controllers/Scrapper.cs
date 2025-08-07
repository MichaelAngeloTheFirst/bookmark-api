namespace BookmarkAI_API.Controllers;

using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using Html2Markdown;
using HtmlAgilityPack;

[ApiController]
[Route("[controller]")]
public class Scrapper : ControllerBase
{
    private readonly HttpClient _httpClient;

        public Scrapper()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36"
            );
            _httpClient.DefaultRequestHeaders.Accept.ParseAdd("text/html,application/xhtml+xml,application/xml");
        }

        [HttpGet]
        public async Task<IActionResult> GetMarkdown([FromQuery] string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return BadRequest("URL is required.");
            
            var (llmExists, llmContent) = await CheckLlmTxtAsync(url);
            if (llmExists)
            {
                // Use the content of llm.txt (for example, return it)
                return Content(llmContent ?? string.Empty, "text/plain");
            }
            
            Console.WriteLine("Need to fetch the site and convert to Markdown.");
            
            try
            {
                var html = await _httpClient.GetStringAsync(url);

                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                // Remove all class attributes
                foreach (var node in doc.DocumentNode.SelectNodes("//*[@class]") ?? new HtmlNodeCollection(null))
                {
                    node.Attributes["class"].Remove();
                }

                var cleanedHtml = doc.DocumentNode.OuterHtml;

                var converter = new Converter();
                var markdown = converter.Convert(cleanedHtml);

                return Content(markdown, "text/plain");
            }
            catch
            {
                return BadRequest("Failed to fetch or convert the site.");
            }
        }

        private async Task<(bool exists, string? content)> CheckLlmTxtAsync(string url)
        {
            try
            {
                var uri = new Uri(url);
                var baseUrl = $"{uri.Scheme}://{uri.Host}";
                var llmUrl = $"{baseUrl}/llms.txt";
                Console.Out.WriteLine($"Checking llms.txt at: {llmUrl}");
                var response = await _httpClient.GetAsync(llmUrl);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return (true, content);
                }

                return (false, null);
            }
            catch
            {
                return (false, null);
            }
        }
    
    
}