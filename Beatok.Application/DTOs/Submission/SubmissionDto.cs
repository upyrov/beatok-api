using Beatok.Application.DTOs.User;

namespace Beatok.Application.DTOs.Submission;

public record SubmissionDto
{
    public Guid Id { get; set; }
    public required string Value { get; set; }
    public required UserDto User { get; set; }
    public Guid LobbyId { get; set; }
}