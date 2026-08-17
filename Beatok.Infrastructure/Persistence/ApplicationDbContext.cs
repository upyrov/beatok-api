using Beatok.Application.Interfaces;
using Beatok.Domain.Entities;
using Beatok.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Beatok.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options), IApplicationDbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<Kit> Kits { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Sound> Sounds { get; set; }
      
    public DbSet<Lobby> Lobbies { get; set; }
    public DbSet<Participation> Participation { get; set; }
    public DbSet<Submission> Submissions { get; set; }
    public DbSet<Score> Scores { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<LobbyPlaybackItem> LobbyPlaybackItems { get; set; }
    
    private const double Mu = 25.0;
    private const double Sigma = 8.333;
    private const double Rating = 0;
    
    public async Task EnsureUserExistsAsync(string userId, string name, bool isAnonymous)
    {
        DateTime? lastActiveAt = isAnonymous ? DateTime.UtcNow : null;
        
        await Database.ExecuteSqlInterpolatedAsync($""" 
                                                              INSERT INTO "Users"
                                                                  ("Id", "Name", "IsAnonymous", "LastActiveAt", "Mu", "Sigma", "Rating")
                                                              VALUES
                                                                  ({userId}, {name}, {isAnonymous}, {lastActiveAt}, {Mu}, {Sigma}, {Rating})
                                                              ON CONFLICT ("Id") DO NOTHING;
                                                              """);
    } 

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserEntityTypeConfiguration).Assembly);
    }
}