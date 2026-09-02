namespace Beatok.Application.DTOs.User;

public record UserDto
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public double Rating { get; set; }
    public PictureDto? Picture { get; set; }
}