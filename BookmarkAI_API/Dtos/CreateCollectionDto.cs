namespace BookmarkAI_API.Dtos;

public class CreateCollectionDto
{
    public string UserId { get; set; }
    
    public int PublicCollectionId { get; set; }
    public string Name { get; set; } = string.Empty;
}
