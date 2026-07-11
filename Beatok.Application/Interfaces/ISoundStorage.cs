namespace Beatok.Application.Interfaces;

public interface ISoundStorage
{
    Task<IReadOnlyList<string>> ListSoundKeysAsync();
    string GeneratePresignedSoundUrl(string key, TimeSpan expires);
}