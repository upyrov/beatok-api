using Beatok.Application.DTOs.Score;

namespace Beatok.Application.Interfaces.Services;

public interface IScoreService
{
    Task CreateAsync(Guid userId, Guid lobbyId, CreateScoreDto dto);
    Task UpdateValueAsync(Guid userId, Guid lobbyId, Guid scoreId, ScoreUpdateDto dto);
}