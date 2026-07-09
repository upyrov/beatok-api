using Beatok.Domain.Entities;

namespace Beatok.Application.Interfaces.Repositories;

public interface ILobbyRepository
{
    Task AddAsync(Lobby lobby);
}