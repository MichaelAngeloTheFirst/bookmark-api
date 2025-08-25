namespace BookmarkAI_API.Models;

public class UserUrl
{
    public Guid Id { get; set; }
    public required string UserId { get; set; } = string.Empty;
    public required string Url { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}