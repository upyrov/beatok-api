using Beatok.Domain.Entities;
using Beatok.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Beatok.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Kit> Kits { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Sound> Sounds { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserEntityTypeConfiguration).Assembly);
    }
}