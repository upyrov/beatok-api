namespace Beatok.Application.DTOs.Sound;

public record SoundUploadDto
{
    public required string UploadUrl { get; set; }
    public required string FileKey { get; set; }
}