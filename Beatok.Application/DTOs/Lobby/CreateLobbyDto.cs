namespace Beatok.Application.DTOs.Lobby;

public record CreateLobbyDto
{
    public required string Name { get; set; }
    public short ParticipantLimit { get; set; }
    public TimeSpan SubmissionTime { get; set; }
    public Guid GenreId { get; set; }
}