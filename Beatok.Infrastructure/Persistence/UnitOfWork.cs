using Beatok.Application.Interfaces;
using Beatok.Application.Interfaces.Repositories;

namespace Beatok.Infrastructure.Persistence;

public class UnitOfWork(ApplicationDbContext context, IUserRepository users,
    IGenreRepository genres, IRefreshTokenRepository refreshTokens,
    IKitRepository kits, ICategoryRepository categories, ISoundRepository sounds,
    ILobbyRepository lobbies, IParticipationRepository participations): IUnitOfWork
{
    public IUserRepository Users { get; } = users;
    public IGenreRepository Genres { get; } = genres;
    public IRefreshTokenRepository RefreshTokens { get; } = refreshTokens;
    
    public IKitRepository Kits { get; } = kits;
    public ICategoryRepository Categories { get; } = categories;
    public ISoundRepository Sounds { get; } = sounds;
    
    public ILobbyRepository Lobbies { get; } = lobbies;
    public IParticipationRepository Participation { get; } = participations;

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}