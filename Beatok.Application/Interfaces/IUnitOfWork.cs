using Beatok.Application.Interfaces.Repositories;

namespace Beatok.Application.Interfaces;

public interface IUnitOfWork
{
    public IUserRepository Users { get; }
    public IGenreRepository Genres { get; }
    public IRefreshTokenRepository RefreshTokens { get; }
    public ILobbyRepository Lobbies { get; }
    public IParticipationRepository Participation { get; }
    
    public Task SaveChangesAsync();
}