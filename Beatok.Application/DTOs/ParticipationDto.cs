using Beatok.Application.DTOs.User;

namespace Beatok.Application.DTOs;

public record ParticipationDto
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public bool IsConnected { get; set; } = true;
    public required UserDto User { get; set; }
}