namespace Beatok.Application.DTOs.User;

public record PictureUploadDto
{
    public required string UploadUrl { get; set; }
    public required string FileKey { get; set; }
}