using Beatok.Application.Interfaces.Repositories;

namespace Beatok.Application.Interfaces;

public interface IUnitOfWork
{
    public IUserRepository Users { get; }
    public IGenreRepository Genres { get; }
    public IRefreshTokenRepository RefreshTokens { get; }
    
    public Task SaveChangesAsync();
}