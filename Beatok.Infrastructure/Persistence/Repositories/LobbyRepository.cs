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
            query = query.Where(l => l.Name.ToLower()
                .Contains(filter.Name.Trim().ToLower()));
        }

        if (filter.GenreId.HasValue)
        {
            query = query.Where(l => l.GenreId == filter.GenreId);
        }
        
        return await query.ToListAsync();
    }
}