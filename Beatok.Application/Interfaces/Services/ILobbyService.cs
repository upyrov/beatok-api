using Beatok.Application.DTOs;
using Beatok.Application.DTOs.Lobby;

namespace Beatok.Application.Interfaces.Services;

public interface ILobbyService
{
    Task<Guid> CreateAsync(CreateLobbyDto dto, string ownerId);
    Task<IEnumerable<LobbyDto>> GetAllAsync(LobbyFilterDto filter, string? userIdStr);
    Task<List<ArchivedLobbyDto>> GetByUserIdAsync(string userId, DateTime date);
    Task KickAsync(Guid lobbyId, string userId, string targetUserId);
    Task<DetailedLobbyDto> JoinAsync(Guid lobbyId, string userId, string connectionId);
    Task LeaveAsync(Guid lobbyId, string userId);
    Task DisconnectAsync(string connectionId);
    Task HandleDisconnectTimeoutAsync(Guid lobbyId, string userId);
    Task SendMessageAsync(Guid lobbyId, string userId, string content);
}