using Beatok.Domain.Entities;

namespace Beatok.Application.DTOs.Lobby;

public class CreateLobbyDto
{
    public string Name { get; set; } = string.Empty;
    public Guid GenreId { get; set; }
    public short ParticipantLimit { get; set; }
    public TimeSpan SubmissionTimeLimit { get; set; }
    public TimeSpan VotingTimeLimit { get; set; }
}