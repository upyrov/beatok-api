using Beatok.Application.DTOs;
using Beatok.Application.DTOs.Lobby;
using Beatok.Domain.Entities;

namespace Beatok.Application.Interfaces.Services;

public interface ILobbyService
{
    Task CreateAsync(CreateLobbyDto dto, Guid ownerId);
    Task<IEnumerable<LobbyDto>> GetAllAsync(LobbyFilterDto filter);
    Task StartAsync(Guid lobbyId, Guid userId);
    Task TransitionToVotingAsync(Guid lobbyId);
    Task TransitionToEndAsync(Guid lobbyId);
    Task JoinAsync(Guid lobbyId, Guid userId);
    Task LeaveAsync(Guid lobbyId, Guid userId);
    Task SetConnectionIdAsync(Guid lobbyId, Guid userId, string connectionId);
    Task DisconnectAsync(string connectionId);
    Task TryFinishVoting(Lobby lobby);
}