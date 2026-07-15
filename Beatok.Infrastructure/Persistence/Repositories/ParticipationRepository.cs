using Beatok.Application.Interfaces.Repositories;
using Beatok.Domain.Entities;

namespace Beatok.Infrastructure.Persistence.Repositories;

public class ParticipationRepository(ApplicationDbContext context): IParticipationRepository
{
    public async Task AddAsync(Participation participation)
    {
        await context.Participation.AddAsync(participation);
    }
    
    public void Delete(Participation participation)
    {
        context.Participation.Remove(participation);
    }
}