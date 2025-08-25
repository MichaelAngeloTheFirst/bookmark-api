using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookmarkAI_API.Data;
using BookmarkAI_API.Models;
using BookmarkAI_API.Dtos;

namespace BookmarkAI_API.Controllers;



[ApiController]
[Route("api/[controller]")]
public class UrlsController : ControllerBase
{
    private readonly AppDbContext _db;

    public UrlsController(AppDbContext db)
    {
        _db = db;
    }
    
    [HttpPost]
    public async Task<IActionResult> SaveUrl([FromBody] SaveUrlRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                     ?? User.FindFirst("subject")?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User not found");

        var userUrl = new UserUrl
        {
            UserId = userId,
            Url = request.Url
        };

        _db.UserUrls.Add(userUrl);
        await _db.SaveChangesAsync();

        return Ok(new { message = "URL saved", id = userUrl.Id });
    }
    
    [HttpGet]
    public async Task<IActionResult> GetUserUrls()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                     ?? User.FindFirst("subject")?.Value;
        Console.WriteLine($"UserId: {User.Claims}");
        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User not found");

        var urls = await _db.UserUrls
            .Where(u => u.UserId == userId)
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();

        return Ok(urls);
    }
}