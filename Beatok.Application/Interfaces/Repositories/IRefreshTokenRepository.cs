using Beatok.Domain.Entities;

namespace Beatok.Application.Interfaces.Repositories;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken token);
    Task<RefreshToken?> GetByHashAsync(string token);
    Task<int> DeleteExpiredAsync();
    void Delete(RefreshToken token);
}