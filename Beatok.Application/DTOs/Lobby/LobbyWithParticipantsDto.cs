using Beatok.Application.DTOs.Genre;
using Beatok.Application.DTOs.User;
using Beatok.Domain.Entities;

namespace Beatok.Application.DTOs.Lobby;

public record LobbyWithParticipantsDto
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public required GenreDto Genre { get; set; }
    public required UserDto Owner { get; set; }
    public int ParticipantLimit { get; set; }
    public LobbyPhase Phase { get; set; } 
    public IEnumerable<UserDto> Participants { get; set; } = [];
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public TimeSpan SubmissionTimeLimit { get; set; }
}