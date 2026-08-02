using Beatok.Application.DTOs.Genre;
using Beatok.Application.DTOs.User;

namespace Beatok.Application.DTOs.Lobby;

public record LobbyDto
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Name { get; set; }
    public int ParticipantCount { get; set; }
    public int ParticipantLimit { get; set; }
    public TimeSpan SubmissionTime { get; set; }
    public DateTime CreatedAt { get; set; }
    public required GenreDto Genre { get; set; }
    public required UserDto Owner { get; set; }
    public bool IsJoined { get; set; }
}