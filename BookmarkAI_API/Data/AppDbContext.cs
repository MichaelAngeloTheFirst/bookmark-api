using BookmarkAI_API.Models;
using Microsoft.EntityFrameworkCore;


namespace BookmarkAI_API.Data;



public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<UserUrl> UserUrls { get; set; }
}