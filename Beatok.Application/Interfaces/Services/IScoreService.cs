using Beatok.Application.DTOs.Score;

namespace Beatok.Application.Interfaces.Services;

public interface IScoreService
{
    Task CreateAsync(Guid userId, Guid lobbyId, CreateScoreDto dto);
}