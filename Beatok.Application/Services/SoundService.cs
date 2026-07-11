using Beatok.Application.Interfaces;
using Beatok.Application.Interfaces.Services;
using Microsoft.Extensions.Caching.Memory;

namespace Beatok.Application.Services;

public class SoundService(ISoundStorage soundStorage, IMemoryCache cache): ISoundService
{
    private readonly Random _random = new();
    
    public async Task RefreshCacheAsync()
    {
        var keys = await soundStorage.ListSoundKeysAsync();
        cache.Set("sound_keys", keys);
    }

    public List<string> GenerateOneShotKit(string genre)
    {
        var allKeys = cache.Get<List<string>>("sound_keys") ?? new List<string>();
        var baseCategoryPath = $"{genre}/one-shots/";

        var kitIds = allKeys
            .Where(k => k.StartsWith(baseCategoryPath) && k != baseCategoryPath)
            .Select(k => k.Replace(baseCategoryPath, "").Split('/')[0])
            .Distinct()
            .ToList();
        
        var randomKitId = kitIds[_random.Next(kitIds.Count)];
        var kitPath = $"{baseCategoryPath}{randomKitId}/";

        var subFolders = allKeys
            .Where(k => k.StartsWith(kitPath) && k != kitPath)
            .Select(k => k.Replace(kitPath, "").Split('/')[0])
            .Distinct()
            .ToList();

        var soundUrls = new List<string>();
        
        foreach (var folder in subFolders)
        {
            var folderPath = $"{kitPath}{folder}/";
            var filesInFolder = allKeys.Where(k => k.StartsWith(folderPath)).ToList();

            if (filesInFolder.Any())
            {
                var randomFile = filesInFolder[_random.Next(filesInFolder.Count)];
            
                soundUrls.Add(soundStorage.GeneratePresignedSoundUrl(randomFile, TimeSpan.FromMinutes(15)));
            }
        }
        return soundUrls;
    }
}