using Amazon.S3;
using Amazon.S3.Model;
using Beatok.Application.Interfaces;

namespace Beatok.Infrastructure;

public class R2StorageSound(IAmazonS3 s3Client): ISoundStorage
{
    public string GeneratePresignedSoundUrl(string key, TimeSpan expires)
    {
        return s3Client.GetPreSignedURL(new GetPreSignedUrlRequest
        {
            BucketName = "beatok",
            Key = key,
            Expires = DateTime.UtcNow.Add(expires)
        });
    }
}