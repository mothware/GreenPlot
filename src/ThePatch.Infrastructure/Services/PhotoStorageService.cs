using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ThePatch.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ThePatch.Infrastructure.Services;

public class PhotoStorageService : IPhotoStorageService
{
    private readonly BlobServiceClient _blobClient;
    private readonly ILogger<PhotoStorageService> _logger;
    private readonly long _quotaBytes;
    private const string ContainerName = "thepatch-photos";

    public PhotoStorageService(IConfiguration config, ILogger<PhotoStorageService> logger)
    {
        _blobClient = new BlobServiceClient(config.GetConnectionString("AzureStorage")
            ?? "UseDevelopmentStorage=true");
        _logger = logger;
        var quotaGb = config.GetValue<double>("Storage:PhotoQuotaGb", 10.0);
        _quotaBytes = (long)(quotaGb * 1024 * 1024 * 1024);
    }

    public async Task<string> UploadAsync(Stream content, string fileName, string contentType, CancellationToken ct = default)
    {
        var container = _blobClient.GetBlobContainerClient(ContainerName);
        await container.CreateIfNotExistsAsync(PublicAccessType.Blob, cancellationToken: ct);

        var blobName = $"{Guid.NewGuid():N}/{fileName}";
        var blob = container.GetBlobClient(blobName);

        await blob.UploadAsync(content, new BlobHttpHeaders { ContentType = contentType }, cancellationToken: ct);
        _logger.LogInformation("Uploaded photo: {BlobName}", blobName);

        return blob.Uri.ToString();
    }

    public async Task DeleteAsync(string url, CancellationToken ct = default)
    {
        var uri = new Uri(url);
        var blobName = string.Join("/", uri.Segments.Skip(2));
        var container = _blobClient.GetBlobContainerClient(ContainerName);
        await container.GetBlobClient(blobName).DeleteIfExistsAsync(cancellationToken: ct);
    }
}
