using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookmarkAI_API.Data;
using BookmarkAI_API.Models;
using BookmarkAI_API.Dtos;

namespace BookmarkAI_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TagsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public TagsController(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    // POST /api/tags/{urlId}/add
    [HttpPost("{urlId}/add")]
    public async Task<IActionResult> AddTag(int urlId, [FromBody] string tagName)
    {
        var userUrl = await _db.Bookmark
            .Include(u => u.BookmarkTags)
            .ThenInclude(ut => ut.Tag)
            .FirstOrDefaultAsync(u => u.BookmarkId == urlId);

        if (userUrl == null) return NotFound("URL not found");

        var tag = await _db.Tags.FirstOrDefaultAsync(t => t.Name == tagName);
        if (tag == null)
        {
            tag = new Tag { Name = tagName };
            _db.Tags.Add(tag);
        }

        if (!userUrl.BookmarkTags.Any(ut => ut.Tag.Name == tagName))
        {
            userUrl.BookmarkTags.Add(new BookmarkTag { Bookmark = userUrl, Tag = tag });
            await _db.SaveChangesAsync();
        }

        return Ok(new { urlId, tagName });
    }

    // GET /api/tags/{tagName}/urls
    [HttpGet("{tagName}/urls")]
    public async Task<IActionResult> GetUrlsForTag(string tagName, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var tag = await _db.Tags
            .Include(t => t.BookmarkTags)
            .ThenInclude(ut => ut.Bookmark)
            .FirstOrDefaultAsync(t => t.Name == tagName);

        if (tag == null) return NotFound("Tag not found");

        var totalCount = tag.BookmarkTags.Count;

        var urls = tag.BookmarkTags
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(ut => ut.Bookmark)
            .ToList();

        var dtoList = _mapper.Map<List<BookmarkResponseDto>>(urls);

        return Ok(new
        {
            Tag = tagName,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            Urls = dtoList
        });
    }

    // GET /api/tags
    [HttpGet]
    public async Task<IActionResult> GetAllTags([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var query = _db.Tags.OrderBy(t => t.Name);

        var totalCount = await query.CountAsync();
        var tags = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        var dtoList = _mapper.Map<List<TagDto>>(tags);

        return Ok(new
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            Tags = dtoList
        });
    }
}
