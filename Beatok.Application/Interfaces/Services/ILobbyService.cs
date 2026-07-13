using Beatok.Application.DTOs;
using Beatok.Application.DTOs.Lobby;

namespace Beatok.Application.Interfaces.Services;

public interface ILobbyService
{
    Task CreateAsync(CreateLobbyDto dto, Guid ownerId);
    Task<IEnumerable<LobbyDto>> GetAllAsync(LobbyFilterDto filter);
    Task StartLobbyAsync(Guid lobbyId, Guid userId);
    Task TransitionToVotingAsync(Guid lobbyId);
}