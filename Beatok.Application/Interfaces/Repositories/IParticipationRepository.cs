using Beatok.Domain.Entities;

namespace Beatok.Application.Interfaces.Repositories;

public interface IParticipationRepository
{
    Task AddAsync(Participation participation);
    void Delete(Participation participation);
    Task<List<Participation>> GetByConnectionIdAsync(string connectionId);
}