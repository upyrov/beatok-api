namespace Beatok.Application.Interfaces.Services;

public interface ISoundService
{
    Task RefreshCacheAsync();
    List<string> GenerateOneShotKit(string genre);
}