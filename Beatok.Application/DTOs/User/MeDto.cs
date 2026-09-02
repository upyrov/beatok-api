using Beatok.Domain.Entities;

namespace Beatok.Application.DTOs.User;

public record MeDto
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public double Rating { get; set; }
    public PictureDto? Picture { get; set; }
    public UserRole Role { get; set; }
    public bool IsAnonymous { get; set; }
}