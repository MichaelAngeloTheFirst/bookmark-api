namespace BookmarkAI_API.Dtos;

public class BookmarkResponseDto
{
    public int Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public string BookmarkId { get; set; } = string.Empty;
    public string? Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public List<string> Tags { get; set; } = new();
    public int? CollectionId { get; set; }

}