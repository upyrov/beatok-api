using Beatok.Application.DTOs.Score;

namespace Beatok.Application.Interfaces.Services;

public interface IScoreService
{
    Task<Guid> CreateAsync(string userId, Guid lobbyId, CreateScoreDto dto);
    Task UpdateValueAsync(string userId, Guid lobbyId, Guid scoreId, ScoreUpdateDto dto);
}