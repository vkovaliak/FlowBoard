using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using FlowBoard.Application.Abstractions;
using FlowBoard.Infrastructure.Configurations;
using Microsoft.Extensions.Options;

namespace FlowBoard.Infrastructure.Storage;

public class AzureBlobStorageService : IFileStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly AzureBlobOptions _options;

    public AzureBlobStorageService(IOptions<AzureBlobOptions> options)
    {
        _options = options.Value;
        
        _blobServiceClient = new BlobServiceClient(_options.ConnectionString);
    }

    public async Task<string> UploadAsync(
        Stream fileStream, string fileName, string containerName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

        await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

        var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";

        var blobClient = containerClient.GetBlobClient(uniqueFileName);

        fileStream.Position = 0;
        await blobClient.UploadAsync(fileStream, new BlobHttpHeaders 
        { 
            ContentType = GetContentType(fileName) 
        });

        return blobClient.Uri.ToString();
    }

    public async Task DeleteAsync(string fileUrl)
    {
        if (string.IsNullOrWhiteSpace(fileUrl)) return;

        var blobUriBuilder = new BlobUriBuilder(new Uri(fileUrl));
        
        var containerClient = _blobServiceClient.GetBlobContainerClient(
            blobUriBuilder.BlobContainerName);
            
        var blobClient = containerClient.GetBlobClient(blobUriBuilder.BlobName);

        await blobClient.DeleteIfExistsAsync();
    }

    private string GetContentType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".png" => "image/png",
            ".jpg" => "image/jpeg",
            ".jpeg" => "image/jpeg",
            ".gif" => "image/gif",
            ".pdf" => "application/pdf",
            ".txt" => "text/plain",
            _ => "application/octet-stream"
        };
    }
}