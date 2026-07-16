namespace Beatok.Application.Interfaces;

public interface IStorage
{
    string GeneratePresignedSoundUrl(string key, TimeSpan expires);
}