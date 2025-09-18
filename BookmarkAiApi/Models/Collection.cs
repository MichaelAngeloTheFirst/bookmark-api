using System.ComponentModel.DataAnnotations;

namespace BookmarkAiApi.Models;

public class Collection
{
    public int CollectionId { get; set; }

    public required string UserId { get; set; }
    
    [MaxLength(256)]
    public required string Name { get; set; } = string.Empty;
    
    public List<Bookmark> Bookmarks { get; set; } = new();
    
    
    public Guid? ParentCollectionId { get; set; }
    public Collection ParentCollection { get; set; }
    
    public List<Collection> SubCollections { get; set; } = new();
}