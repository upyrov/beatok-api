using Beatok.Application.DTOs;
using Beatok.Application.Interfaces.Repositories;
using Beatok.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Beatok.Infrastructure.Persistence.Repositories;

public class LobbyRepository(ApplicationDbContext context): ILobbyRepository
{
    public async Task AddAsync(Lobby lobby)
    {
        await context.Lobbies.AddAsync(lobby);
    }

    public async Task<IEnumerable<Lobby>> GetFilteredAsync(LobbyFilterDto filter)
    {
        var query = context.Lobbies.AsQueryable();
        query = query.Where(l => l.Phase == LobbyPhase.NotStarted 
                                 && l.Participants.Count < l.ParticipantLimit);
        
        if (!string.IsNullOrEmpty(filter.Name))
        {
            query = query.Where(l => l.Name.Contains(filter.Name.Trim(), StringComparison.CurrentCultureIgnoreCase));
        }

        if (filter.GenreId.HasValue)
        {
            query = query.Where(l => l.GenreId == filter.GenreId);
        }
        
        return await query.ToListAsync();
    }

    public async Task<Lobby?> GetByIdAsync(Guid id)
    {
        return await context.Lobbies
            .Include(l => l.Participants)
                .ThenInclude(p => p.User)
            .Include(l => l.Participants)
                .ThenInclude(p => p.Submissions)
                    .ThenInclude(s => s.Scores)
            .FirstOrDefaultAsync(l => l.Id == id);
    }

    public void Delete(Lobby lobby)
    {
        context.Lobbies.Remove(lobby);
    }
}