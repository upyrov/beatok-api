using Beatok.Application.Interfaces.Repositories;
using Beatok.Domain.Entities;

namespace Beatok.Infrastructure.Persistence.Repositories;

public class ScoreRepository(ApplicationDbContext context): IScoreRepository
{
    public async Task CreateAsync(Score score)
    {
        await context.Scores.AddAsync(score);
    }
}