using System.Collections.ObjectModel;
using System.Security.Claims;
using AutoMapper;
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
    private readonly IMapper _mapper;

    public CollectionsController(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }
    
    
    [HttpPost("collections")]
    public async Task<ActionResult<CollectionDto>> CreateCollection(CreateCollectionDto dto)
    {
        var collection = new Collection
        {
            UserId = dto.UserId,
            Name = dto.Name
        };

        _db.Collections.Add(collection);
        await _db.SaveChangesAsync();
        
        var result = new CollectionDto
        {
            CollectionId = collection.CollectionId,
            UserId = collection.UserId,
            Name = collection.Name,
            Bookmarks = new List<BookmarkResponseDto>()
        };

        return CreatedAtAction(nameof(GetCollection), new { id = collection.CollectionId }, result);
    }
    
    [HttpGet( "collections")]
    public async Task<IActionResult> GetCollections(int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                     ?? User.FindFirst("subject")?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User not found");
        
        var collection = await _db.Collections
            .Include(c => c.Bookmarks)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (collection == null) return NotFound();

        return Ok(_mapper.Map<CollectionDto>(collection));
    }
    
    
    
    [HttpGet( "collections/{id}")]
    public async Task<ActionResult<CollectionDto>> GetCollection(int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                     ?? User.FindFirst("subject")?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User not found");
        
        var collection = await _db.Collections
            .Include(c => c.Bookmarks)
            .FirstOrDefaultAsync(c => c.CollectionId == id && c.UserId == userId);

        if (collection == null) return NotFound();

        return new CollectionDto
        {
            CollectionId = collection.CollectionId,
            UserId = collection.UserId,
            Name = collection.Name,
            Bookmarks= collection.Bookmarks.Select(u => new BookmarkResponseDto
            {
                Id = u.BookmarkId,
                Url = u.Url,
            }).ToList()
        };
    }
    
}