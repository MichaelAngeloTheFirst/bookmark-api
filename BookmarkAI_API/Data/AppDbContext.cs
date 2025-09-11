using BookmarkAI_API.Models;
using Microsoft.EntityFrameworkCore;


namespace BookmarkAI_API.Data;



public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Bookmark> Bookmark { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<BookmarkTag> BookmarkTags { get; set; }
    public DbSet<Collection> Collections { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BookmarkTag>()
            .HasKey(x => new { x.BookmarkId, x.TagId });

        modelBuilder.Entity<BookmarkTag>()
            .HasOne(x => x.Bookmark)
            .WithMany(u => u.BookmarkTags)
            .HasForeignKey(x => x.BookmarkId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<BookmarkTag>()
            .HasOne(x => x.Tag)
            .WithMany(t => t.BookmarkTags)
            .HasForeignKey(x => x.TagId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Collection>()
            .HasMany(c => c.Bookmarks)
            .WithOne(u => u.Collection)
            .HasForeignKey(u => u.CollectionId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);
        
        
        modelBuilder.Entity<Collection>()
            .HasMany(c => c.SubCollections)
            .WithOne(c => c.ParentCollection)
            .HasForeignKey(c => c.ParentCollectionId)
            .OnDelete(DeleteBehavior.Cascade);
        
        base.OnModelCreating(modelBuilder);
    }
    
    
    
    
}