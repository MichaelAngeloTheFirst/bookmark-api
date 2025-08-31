namespace BookmarkAI_API.Dtos;

public class TagResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<string> Bookmarks { get; set; } = new();
}