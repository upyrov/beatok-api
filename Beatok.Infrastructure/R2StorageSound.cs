using Amazon.S3;
using Amazon.S3.Model;
using Beatok.Application.Interfaces;

namespace Beatok.Infrastructure;

public class R2StorageSound(IAmazonS3 s3Client): ISoundStorage
{
    public async Task<IReadOnlyList<string>> ListSoundKeysAsync()
    {
        var response = await s3Client.ListObjectsV2Async(new ListObjectsV2Request
        {
            BucketName = "sounds"
        });
        var allFiles = response.S3Objects.Select(o => o.Key).ToList();
        return allFiles;
    }

    public string GeneratePresignedSoundUrl(string key, TimeSpan expires)
    {
        return s3Client.GetPreSignedURL(new GetPreSignedUrlRequest
        {
            BucketName = "sounds",
            Key = key,
            Expires = DateTime.UtcNow.Add(expires)
        });
    }
}