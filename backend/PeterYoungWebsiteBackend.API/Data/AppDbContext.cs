using Microsoft.EntityFrameworkCore;

namespace PeterYoungWebsiteBackend.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Define your DbSets (tables) here as you build models, e.g.:
    // public DbSet<YourEntity> Entities => Set<YourEntity>();
}

