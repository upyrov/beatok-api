namespace Beatok.Application.Interfaces.Services;

public interface ILobbyLifecycleService
{
    Task StartAsync(Guid lobbyId, string userId);
    Task TransitionToVotingAsync(Guid lobbyId);
    Task StartPlaybackAsync(Guid lobbyId);
    
    Task TransitionToEndAsync(Guid lobbyId);
    Task PlayNextItemAsync(Guid lobbyId, int order);
}