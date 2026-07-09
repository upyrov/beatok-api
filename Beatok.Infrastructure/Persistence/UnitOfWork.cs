using Beatok.Application.Interfaces;
using Beatok.Application.Interfaces.Repositories;

namespace Beatok.Infrastructure.Persistence;

public class UnitOfWork(ApplicationDbContext context, IUserRepository users,
    IGenreRepository genres, IRefreshTokenRepository refreshTokens,
    ILobbyRepository lobbies): IUnitOfWork
{
    public IUserRepository Users { get; } = users;
    public IGenreRepository Genres { get; } = genres;
    public IRefreshTokenRepository RefreshTokens { get; } = refreshTokens;
    public ILobbyRepository Lobbies { get; } = lobbies;

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}