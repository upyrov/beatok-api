namespace Beatok.Application.Interfaces;

public interface IStorage
{
    string GeneratePresignedUrl(string key, TimeSpan expires);
    string GeneratePresignedUploadUrl(string key, TimeSpan expires, string? contentType = null);
    Task DeleteFileAsync(string key);
}