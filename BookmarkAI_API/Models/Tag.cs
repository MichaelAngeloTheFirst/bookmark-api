namespace BookmarkAI_API.Models;

public class Tag
{
    public int TagId { get; set; }
    public required string Name { get; set; } = string.Empty;
    
    public ICollection<BookmarkTag> BookmarkTags { get; set; } = new List<BookmarkTag>();
}