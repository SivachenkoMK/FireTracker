using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using FireTracker.Api.Options;
using Microsoft.Extensions.Options;

namespace FireTracker.Api.Services;

public class StorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly ImageStorageConfiguration _imageStorageConfiguration;

    public StorageService(IOptions<ImageStorageConfiguration> configuration)
    {
        _imageStorageConfiguration = configuration.Value;
        _blobServiceClient = new BlobServiceClient(configuration.Value.ConnectionString);
    }

    public async Task<string> Upload(IFormFile file, Guid incidentId, CancellationToken token)
    {
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("File cannot be null or empty.", nameof(file));
        }

        var fileExtension = Path.GetExtension(file.FileName);
        if (string.IsNullOrWhiteSpace(fileExtension))
        {
            throw new ArgumentException("File must have an extension.", nameof(file));
        }

        var blobName = $"{incidentId}{fileExtension}";

        var containerClient = _blobServiceClient.GetBlobContainerClient(_imageStorageConfiguration.ContainerName);

        var blobClient = containerClient.GetBlobClient(blobName);

        await using (var stream = file.OpenReadStream())
        {
            await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = file.ContentType }, cancellationToken: token);
        }

        var expirationTime = DateTimeOffset.UtcNow.AddDays(1);
        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = _imageStorageConfiguration.ContainerName,
            BlobName = blobName,
            Resource = "b",
            ExpiresOn = expirationTime
        };
        sasBuilder.SetPermissions(BlobSasPermissions.Read | BlobSasPermissions.Delete);

        // TODO: Might want to rework this to only send the path, and not sas token - for security purposes
        return blobClient.GenerateSasUri(sasBuilder).ToString();
    }
}
