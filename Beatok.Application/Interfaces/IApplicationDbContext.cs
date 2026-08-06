using Beatok.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Beatok.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Genre> Genres { get; }
    DbSet<Kit> Kits { get; }
    DbSet<Category> Categories { get; }
    DbSet<Sound> Sounds { get; }
      
    DbSet<Lobby> Lobbies { get; }
    DbSet<Participation> Participation { get; }
    DbSet<Submission> Submissions { get; }
    
    DbSet<Score> Scores { get; }
    DbSet<Comment> Comments { get; }
    
    Task EnsureUserExistsAsync(string userId, string name, bool isAnonymous);
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}