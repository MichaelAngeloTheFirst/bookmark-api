using System.Collections.ObjectModel;
using BookmarkAI_API.Data;
using BookmarkAI_API.Dtos;
using BookmarkAI_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace BookmarkAI_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CollectionsController : ControllerBase
{
    private readonly AppDbContext _db;

    public CollectionsController(AppDbContext db)
    {
        _db = db;
    }
    
    
    [HttpPost]
    public async Task<ActionResult<CollectionDto>> CreateCollection(CreateCollectionDto dto)
    {
        var collection = new Collection
        {
            Name = dto.Name
        };

        _db.Collections.Add(collection);
        await _db.SaveChangesAsync();
        
        var result = new CollectionDto
        {
            Id = collection.CollectionId,
            Name = collection.Name,
            Bookmarks = new List<BookmarkResponseDto>()
        };

        return CreatedAtAction(nameof(GetCollection), new { id = collection.CollectionId }, result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CollectionDto>> GetCollection(int id)
    {
        var collection = await _db.Collections
            .Include(c => c.Bookmarks)
            .FirstOrDefaultAsync(c => c.CollectionId == id);

        if (collection == null) return NotFound();

        return new CollectionDto
        {
            Id = collection.CollectionId,
            Name = collection.Name,
            Bookmarks= collection.Bookmarks.Select(u => new BookmarkResponseDto
            {
                Id = u.BookmarkId,
                Url = u.Url,
            }).ToList()
        };
    }
}