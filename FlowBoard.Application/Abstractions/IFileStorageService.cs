namespace FlowBoard.Application.Abstractions;

public interface IFileStorageService
{
    Task<string> UploadAsync(Stream fileStream, string fileName, string containerName);

    Task DeleteAsync(string fileUrl);

    Task<IReadOnlyList<string>> ListBlobUrlsAsync(string containerName);
}