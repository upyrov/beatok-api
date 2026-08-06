using Beatok.Application.Interfaces.Services;
using Beatok.Domain.Entities;
using OpenSkillSharp.Models;
using OpenSkillSharp.Rating;

namespace Beatok.Application.Services;

public class MmrService: IMmrService
{
    public Dictionary<string, (double NewMu, double NewSigma, double RatingChange)> CalculateRatings(Lobby lobby)
    { 
        var participants = lobby.Participants.ToList();
        if (participants.Count <= 1)
            return new Dictionary<string, (double, double, double)>();

        var rankedParticipants = participants
            .Select(p => new
            {
                Participant = p,
                TotalScore = p.Submissions.SelectMany(s => s.Scores).Sum(s => s.Value),
                LastVote = p.Submissions.SelectMany(s => s.Scores).Any()
                    ? p.Submissions.SelectMany(s => s.Scores).Max(s => s.CreatedAt)
                    : DateTime.MinValue
            })
            .OrderByDescending(x => x.TotalScore)
            .ThenBy(x => x.LastVote)
            .ToList();

        var model = new PlackettLuce();
        var teams = new List<ITeam>();

        foreach (var item in rankedParticipants)
        {
            var user = item.Participant.User;
            IRating rating = model.Rating(mu: user!.Mu, sigma: user.Sigma);
            teams.Add(new Team { Players = [rating] });
        }

        List<ITeam> updatedTeams = model.Rate(teams).ToList();

        var results = new Dictionary<string, (double NewMu, double NewSigma, double RatingChange)>();

        for (int i = 0; i < rankedParticipants.Count; i++)
        {
            var item = rankedParticipants[i];
            var user = item.Participant.User;
            var userId = user!.Id;


            IRating newRating = updatedTeams[i].Players.First();

            double ratingChange = newRating.Mu - user.Mu;

            results[userId] = (newRating.Mu, newRating.Sigma, ratingChange);
        }

        return results;
    }
}