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
public class BookmarkController(AppDbContext db, IMapper mapper) : ControllerBase
{
    [HttpPost("bookmarks")]
    public async Task<IActionResult> CreateBookmark([FromBody] SaveBookmarkRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                     ?? User.FindFirst("subject")?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User not found");

        var userBookmark = new Bookmark
        {
            UserId = userId,
            Url = request.Url,
        };

        db.Bookmark.Add(userBookmark);
        await db.SaveChangesAsync();

        return Ok(new { message = "URL saved", id = userBookmark.BookmarkId });
    }

    [HttpGet("bookmarks")]
    public async Task<IActionResult> GetUserBookmarks()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                     ?? User.FindFirst("subject")?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User not found");

        var urls = await db.Bookmark
            .Include(u => u.BookmarkTags)
            .ThenInclude(ut => ut.Tag)
            .Where(u => u.UserId == userId)
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();

        return Ok(mapper.Map<List<BookmarkResponseDto>>(urls));
    }
    
    
    [HttpPost("bookmarks/with-tags")]
    public async Task<IActionResult> SaveUrlWithTags([FromBody] CreateBookmarkDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                     ?? User.FindFirst("subject")?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User not found");

        var userUrl = new Bookmark
        {
            UserId = userId,
            Url = dto.Url
        };

        foreach (var tagName in dto.Tags.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            var tag = await db.Tags.FirstOrDefaultAsync(t => t.Name == tagName);
            if (tag == null)
            {
                tag = new Tag { Name = tagName };
                db.Tags.Add(tag);
            }

            userUrl.BookmarkTags.Add(new BookmarkTag
            {
                Bookmark = userUrl,
                Tag = tag
            });
        }

        db.Bookmark.Add(userUrl);
        await db.SaveChangesAsync();

        return Ok(mapper.Map<BookmarkResponseDto>(userUrl));
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUrl(int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? User.FindFirst("subject")?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User not found");
        var bookmark = await db.Bookmark.FirstOrDefaultAsync(b => b.BookmarkId == id && b.UserId == userId);
        if (bookmark == null) return NotFound("Bookmark not found");
        db.Bookmark.Remove(bookmark);
        await db.SaveChangesAsync();
        return NoContent();
    }
    
    [HttpGet("bookmarks/{id}/tags")]
    public async Task<IActionResult> GetTagsForBookmark(int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                     ?? User.FindFirst("subject")?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User not found");

        var bookmark = await db.Bookmark
            .Include(b => b.BookmarkTags)
            .ThenInclude(bt => bt.Tag)
            .FirstOrDefaultAsync(b => b.BookmarkId == id && b.UserId == userId);

        if (bookmark == null) return NotFound("Bookmark not found");

        var tags = bookmark.BookmarkTags.Select(bt => bt.Tag.Name).ToList();

        return Ok(mapper.Map<List<string>>(tags));
    }
    
    

}
