using System.ComponentModel.DataAnnotations;

namespace BookmarkAiApi.Models;

public class Bookmark
{
    public int BookmarkId { get; set; }
    public required string UserId { get; set; } = string.Empty;
    
    [MaxLength(1024)]
    public required string Url { get; set; } = string.Empty;
    
    [MaxLength(256)]
    public string? Title { get; set; } = string.Empty;
    
    [MaxLength(1024)]
    public string? Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public ICollection<BookmarkTag>  BookmarkTags { get; set; } = new List<BookmarkTag>();
    
    public int? CollectionId { get; set; }
    public Collection? Collection { get; set; }
}