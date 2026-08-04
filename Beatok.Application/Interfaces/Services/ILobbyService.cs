using Beatok.Application.DTOs;
using Beatok.Application.DTOs.Lobby;
using Beatok.Domain.Entities;

namespace Beatok.Application.Interfaces.Services;

public interface ILobbyService
{
    Task<Guid> CreateAsync(CreateLobbyDto dto, Guid ownerId);
    Task<IEnumerable<LobbyDto>> GetAllAsync(LobbyFilterDto filter, string? userIdStr);
    Task<List<LobbyDto>> GetByUserIdAsync(Guid userId, DateTime date);
    Task StartAsync(Guid lobbyId, Guid userId);
    Task KickAsync(Guid lobbyId, Guid userId, Guid targetUserId);
    Task TransitionToVotingAsync(Guid lobbyId);
    Task TransitionToEndAsync(Guid lobbyId);
    Task<DetailedLobbyDto> JoinAsync(Guid lobbyId, Guid userId, string connectionId);
    Task LeaveAsync(Guid lobbyId, Guid userId);
    Task DisconnectAsync(string connectionId);
    Task HandleDisconnectTimeoutAsync(Guid lobbyId, Guid userId);
    Task TryFinishVotingAsync(Lobby lobby);
    Task SendMessageAsync(Guid lobbyId, Guid userId, string content);
}