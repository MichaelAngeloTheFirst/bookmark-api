using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookmarkAI_API.Data;
using BookmarkAI_API.Dtos;
using BookmarkAI_API.Models;

namespace BookmarkAI_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookmarkController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public BookmarkController(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> SaveUrl([FromBody] SaveBookmarkRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                     ?? User.FindFirst("subject")?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User not found");

        var UserBookmark = new Bookmark
        {
            UserId = userId,
            Url = request.Url,
        };

        _db.Bookmark.Add(UserBookmark);
        await _db.SaveChangesAsync();

        return Ok(new { message = "URL saved", id = UserBookmark.BookmarkId });
    }

    [HttpPost("with-tags")]
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
            var tag = await _db.Tags.FirstOrDefaultAsync(t => t.Name == tagName);
            if (tag == null)
            {
                tag = new Tag { Name = tagName };
                _db.Tags.Add(tag);
            }

            userUrl.BookmarkTags.Add(new BookmarkTag
            {
                Bookmark = userUrl,
                Tag = tag
            });
        }

        _db.Bookmark.Add(userUrl);
        await _db.SaveChangesAsync();

        return Ok(_mapper.Map<BookmarkResponseDto>(userUrl));
    }

    [HttpGet]
    public async Task<IActionResult> GetUserUrls()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                     ?? User.FindFirst("subject")?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User not found");

        var urls = await _db.Bookmark
            .Include(u => u.BookmarkTags)
            .ThenInclude(ut => ut.Tag)
            .Where(u => u.UserId == userId)
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();

        return Ok(_mapper.Map<List<BookmarkResponseDto>>(urls));
    }
    
    
}
