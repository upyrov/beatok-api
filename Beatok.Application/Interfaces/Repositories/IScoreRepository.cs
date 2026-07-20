using Beatok.Domain.Entities;

namespace Beatok.Application.Interfaces.Repositories;

public interface IScoreRepository
{ 
    Task CreateAsync(Score score);
}