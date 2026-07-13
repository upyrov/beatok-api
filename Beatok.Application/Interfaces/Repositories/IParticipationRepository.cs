using Beatok.Domain.Entities;

namespace Beatok.Application.Interfaces.Repositories;

public interface IParticipationRepository
{
    Task AddAsync(Participation participation);
}