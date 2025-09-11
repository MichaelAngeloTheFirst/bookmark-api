namespace BookmarkAI_API.Dtos;

public class CollectionDto
{
    public int CollectionId { get; set; }
    
    public string UserId { get; set; }
    
    public string Name { get; set; } = string.Empty;

    public List<BookmarkResponseDto> Bookmarks { get; set; } = new();
    public List<CollectionDto> SubCollections { get; set; } = new();
}
