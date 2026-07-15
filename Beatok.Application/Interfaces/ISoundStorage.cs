namespace Beatok.Application.Interfaces;

public interface ISoundStorage
{
    string GeneratePresignedSoundUrl(string key, TimeSpan expires);
}