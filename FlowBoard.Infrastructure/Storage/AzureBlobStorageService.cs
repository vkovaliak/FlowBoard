using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using FlowBoard.Application.Abstractions;

namespace FlowBoard.Infrastructure.Storage;

public class AzureBlobStorageService : IFileStorageService
{
    private readonly BlobServiceClient _blobServiceClient;

    public AzureBlobStorageService(BlobServiceClient blobServiceClient)
    {
        _blobServiceClient = blobServiceClient;
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

    public async Task<IReadOnlyList<string>> ListBlobUrlsAsync(
        string containerName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(
            containerName);

        if (!await containerClient.ExistsAsync())
        {
            return [];
        }

        var urls = new List<string>();
        await foreach (var blob in containerClient.GetBlobsAsync())
        {
            var blobClient = containerClient.GetBlobClient(blob.Name);
            urls.Add(blobClient.Uri.ToString());
        }

        return urls.OrderBy(u => u).ToList();
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

    public async Task<string?> DownloadAsync(
        Guid boardId, string containerName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(
            containerName);

        if (!await containerClient.ExistsAsync())
            return null;

        await foreach (var blob in containerClient.GetBlobsAsync())
        {
            if (blob.Name.EndsWith($"{boardId}.json"))
            {
                var blobClient = containerClient.GetBlobClient(blob.Name);
                var content = await blobClient.DownloadContentAsync();
                return content.Value.Content.ToString();
            }
        }

        return null;
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