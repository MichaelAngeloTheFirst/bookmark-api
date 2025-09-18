using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace BookmarkAiApi.Modules;


public class Scrapper
{
    private readonly HttpClient _httpClient;

    public Scrapper()
    {
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (HTML, like Gecko) Chrome/120.0.0.0 Safari/537.36"
        );
        _httpClient.DefaultRequestHeaders.Accept.ParseAdd("text/html,application/xhtml+xml,application/xml");
    }

    public async Task<string> GetHtml(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("URL is required.");

        var (llmExists, llmContent) = await CheckLlmTxtAsync(url);
        if (llmExists)
        {
            return llmContent ?? string.Empty;
        }
        
        try
        {
            var html = await _httpClient.GetStringAsync(url); 
            return html;
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to fetch with error {ex.Message}", ex);
        }
    }

    private async Task<(bool exists, string? content)> CheckLlmTxtAsync(string url)
    {
        try
        {
            var uri = new Uri(url);
            var baseUrl = $"{uri.Scheme}://{uri.Host}";
            var llmUrl = $"{baseUrl}/llms.txt";
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