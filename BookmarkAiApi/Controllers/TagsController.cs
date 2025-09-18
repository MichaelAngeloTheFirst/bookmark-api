using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookmarkAiApi.Data;
using BookmarkAiApi.Dtos;
using BookmarkAiApi.Models;

namespace BookmarkAiApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TagsController(AppDbContext db, IMapper mapper) : ControllerBase
{
    [HttpPost("bookmarks/{bookmarkId}/tags")]
    public async Task<IActionResult> AddTag(int bookmarkId, [FromBody] string tagName)
    {
        var bookmark = await db.Bookmark
            .Include(u => u.BookmarkTags)
            .ThenInclude(ut => ut.Tag)
            .FirstOrDefaultAsync(u => u.BookmarkId == bookmarkId);

        if (bookmark == null) return NotFound("URL not found");

        var tag = await db.Tags.FirstOrDefaultAsync(t => t.Name == tagName);
        if (tag == null)
        {
            tag = new Tag { Name = tagName };
            db.Tags.Add(tag);
        }

        if (!bookmark.BookmarkTags.Any(ut => ut.Tag.Name == tagName))
        {
            bookmark.BookmarkTags.Add(new BookmarkTag { Bookmark = bookmark, Tag = tag });
            await db.SaveChangesAsync();
        }

        return Ok(new { bookmarkId, tagName });
    }
    
    [HttpDelete("bookmarks/{bookmarkId}/tags")]
    public async Task<IActionResult> RemoveTagFromBookmark(int bookmarkId, [FromBody]string tagName)
    {
        var bookmark = await db.Bookmark
            .Include(u => u.BookmarkTags)
            .ThenInclude(ut => ut.Tag)
            .FirstOrDefaultAsync(u => u.BookmarkId == bookmarkId);

        if (bookmark == null) return NotFound("URL not found");

        var bookmarkTag = bookmark.BookmarkTags.FirstOrDefault(ut => ut.Tag.Name == tagName);
        if (bookmarkTag != null)
        {
            bookmark.BookmarkTags.Remove(bookmarkTag);
            await db.SaveChangesAsync();
        }

        return Ok(new { bookmarkId, tagName });
    }
    
    
    
    [HttpGet("tags/{tagName}/bookmarks")]
    public async Task<IActionResult> GetBookmarksForTag(string tagName, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? User.FindFirst("subject")?.Value;

        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User not found");
        
        var tagExists = await db.Tags.AnyAsync(t => t.Name == tagName);
        if (!tagExists)
            return NotFound("Tag not found");
        
        var query = db.BookmarkTags
            .Where(bt => bt.Tag.Name == tagName && bt.Bookmark.UserId == userId)
            .Select(bt => bt.Bookmark)
            .OrderBy(b => b.Title); 

        var totalCount = await query.CountAsync();

        var bookmarks = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var dtoList = mapper.Map<List<BookmarkResponseDto>>(bookmarks);

        return Ok(new
        {
            Tag = tagName,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            bookmarks = dtoList
        });
    }
    
    [HttpPost("tags/{tagName}")]
    public async Task<IActionResult> AddTag(string tagName)
    {
        var existingTag = await db.Tags.FirstOrDefaultAsync(t => t.Name == tagName);
        if (existingTag != null)
            return Conflict("Tag already exists");

        var tag = new Tag { Name = tagName };
        db.Tags.Add(tag);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAllTags), new { id = tag.TagId }, tag);
    }
    
    
    [HttpGet("tags")]
    public async Task<IActionResult> GetAllTags([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var query = db.Tags.OrderBy(t => t.Name);

        var totalCount = await query.CountAsync();

        var tags = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new TagCountDto
            {
                TagId = t.TagId,
                Name = t.Name,
                UsageCount = t.BookmarkTags.Count 
            })
            .ToListAsync();

        return Ok(new
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            Tags = tags
        });
    }
}
