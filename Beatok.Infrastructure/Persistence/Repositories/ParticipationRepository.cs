using Beatok.Application.Interfaces.Repositories;
using Beatok.Domain.Entities;
using Microsoft.EntityFrameworkCore;

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

    public async Task<List<Participation>> GetByConnectionIdAsync(string connectionId)
    {
        return await context.Participation
            .Where(p => p.ConnectionId == connectionId)
            .Include(p => p.Lobby)
            .ThenInclude(l => l!.Participants)
            .Include(p => p.User)
            .ToListAsync();
    }
}