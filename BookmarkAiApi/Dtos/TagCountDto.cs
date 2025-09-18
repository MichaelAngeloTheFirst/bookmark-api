namespace BookmarkAiApi.Dtos;

public class TagCountDto
{
    public int TagId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int UsageCount { get; set; }
}