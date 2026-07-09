using Beatok.Application.Interfaces.Repositories;
using Beatok.Domain.Entities;

namespace Beatok.Infrastructure.Persistence.Repositories;

public class LobbyRepository(ApplicationDbContext context): ILobbyRepository
{
    public async Task AddAsync(Lobby lobby)
    {
        await context.Lobbies.AddAsync(lobby);
    }
}