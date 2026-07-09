using Beatok.Application.DTOs.Lobby;

namespace Beatok.Application.Interfaces.Services;

public interface ILobbyService
{
    Task CreateAsync(CreateLobbyDto dto, Guid ownerId);
}