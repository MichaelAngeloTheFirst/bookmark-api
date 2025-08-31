namespace BookmarkAI_API.Dtos;

public class UpdateBookmarkDto
{
    public string Url { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? Description { get; set; }
    public List<string> Tags { get; set; } = new();
    public int? CollectionId { get; set; }

}