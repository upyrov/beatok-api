namespace Beatok.Application.Interfaces;

public interface IStorage
{
    string GeneratePresignedSoundUrl(string key, TimeSpan expires);
    string GeneratePresignedUploadUrl(string key, TimeSpan expires, string? contentType = null);
}