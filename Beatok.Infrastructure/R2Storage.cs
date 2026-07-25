using Amazon.S3;
using Amazon.S3.Model;
using Beatok.Application.Interfaces;

namespace Beatok.Infrastructure;

public class R2Storage(IAmazonS3 s3Client): IStorage
{
    public string GeneratePresignedUrl(string key, TimeSpan expires)
    {
        return s3Client.GetPreSignedURL(new GetPreSignedUrlRequest
        {
            BucketName = "beatok",
            Key = key,
            Expires = DateTime.UtcNow.Add(expires)
        });
    }

    public string GeneratePresignedUploadUrl(string key, TimeSpan expires, string? contentType = null)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = "beatok",
            Key = key,
            Expires = DateTime.UtcNow.Add(expires),
            Verb = HttpVerb.PUT
        };

        if (!string.IsNullOrEmpty(contentType))
        {
            request.ContentType = contentType;
        }

        return s3Client.GetPreSignedURL(request);
    }
}