namespace BookmarkAI_API.Dtos;

public class CollectionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // Nested Url DTOs
    public List<BookmarkResponseDto> Bookmarks { get; set; } = new();
}
