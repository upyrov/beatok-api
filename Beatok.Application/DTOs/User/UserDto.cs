using Beatok.Domain.Entities;

namespace Beatok.Application.DTOs.User;

public record UserDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public UserRole Role { get; set; }
}