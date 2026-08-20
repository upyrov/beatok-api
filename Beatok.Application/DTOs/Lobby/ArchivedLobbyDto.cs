using Beatok.Application.DTOs.Genre;

namespace Beatok.Application.DTOs.Lobby;

public record ArchivedLobbyDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }

    public required GenreDto Genre { get; set; }

    public int ParticipantCount { get; set; }
    public bool IsWinner { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime SubmissionStartedAt { get; set; }
    public DateTime VotingStartedAt { get; set; }
    public DateTime EndedAt { get; set; }
}