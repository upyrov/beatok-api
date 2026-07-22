using Beatok.Domain.Entities;

namespace Beatok.Application.DTOs.Lobby;

public record CreateLobbyDto
{
    public string Name { get; set; } = string.Empty;
    public Guid GenreId { get; set; }
    public short ParticipantLimit { get; set; }
    public TimeSpan SubmissionTimeLimit { get; set; }
}