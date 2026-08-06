using Beatok.Domain.Entities;

namespace Beatok.Application.Interfaces.Services;

public interface IMmrService
{
    Dictionary<string, (double NewMu, double NewSigma, double RatingChange)> CalculateRatings(Lobby lobby);
}