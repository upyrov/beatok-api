using Beatok.Application.DTOs;
using Beatok.Domain.Entities;

namespace Beatok.Application.Interfaces.Repositories;

public interface ILobbyRepository
{
    Task AddAsync(Lobby lobby);
    Task<IEnumerable<Lobby>> GetFilteredAsync(LobbyFilterDto filter);
    Task<Lobby?> GetByIdAsync(Guid id);
    void Delete(Lobby lobby);
}