namespace Beatok.Application.DTOs;

public record PictureDto
{
    public required string Url { get; set; }
    public required string Key { get; set; }
}