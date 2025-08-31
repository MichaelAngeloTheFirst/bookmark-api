using System.ComponentModel.DataAnnotations;

namespace BookmarkAI_API.Models;

public class Collection
{
    public int CollectionId { get; set; }

    [MaxLength(256)]
    public string Name { get; set; } = string.Empty;
    
    public List<Bookmark> Bookmarks { get; set; } = new();
}