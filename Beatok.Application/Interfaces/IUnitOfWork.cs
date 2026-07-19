using Beatok.Application.Interfaces.Repositories;

namespace Beatok.Application.Interfaces;

public interface IUnitOfWork
{
    public IUserRepository Users { get; }
    public IGenreRepository Genres { get; }
    public IRefreshTokenRepository RefreshTokens { get; }
  
    public IKitRepository Kits { get; }
    public ICategoryRepository Categories { get; }
    public ISoundRepository Sounds { get; }
  
    public ILobbyRepository Lobbies { get; }
    public IParticipationRepository Participations { get; }
    public ISubmissionRepository Submissions { get; }
    public IScoreRepository Scores { get; }

    public Task SaveChangesAsync();
}