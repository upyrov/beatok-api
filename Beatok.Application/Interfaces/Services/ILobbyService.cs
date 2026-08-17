using Beatok.Application.DTOs;
using Beatok.Application.DTOs.Lobby;
using Beatok.Domain.Entities;

namespace Beatok.Application.Interfaces.Services;

public interface ILobbyService
{
    Task<Guid> CreateAsync(CreateLobbyDto dto, string ownerId);
    Task<IEnumerable<LobbyDto>> GetAllAsync(LobbyFilterDto filter, string? userIdStr);
    Task<List<LobbyDto>> GetByUserIdAsync(string userId, DateTime date);
    Task StartAsync(Guid lobbyId, string userId);
    Task KickAsync(Guid lobbyId, string userId, string targetUserId);
    Task TransitionToVotingAsync(Guid lobbyId);
    Task StartPlaybackAsync(Guid lobbyId);
    Task PlayNextItemAsync(Guid lobbyId, int order);
    Task TransitionToEndAsync(Guid lobbyId);
    Task<DetailedLobbyDto> JoinAsync(Guid lobbyId, string userId, string connectionId);
    Task LeaveAsync(Guid lobbyId, string userId);
    Task DisconnectAsync(string connectionId);
    Task HandleDisconnectTimeoutAsync(Guid lobbyId, string userId);
    Task SendMessageAsync(Guid lobbyId, string userId, string content);
}