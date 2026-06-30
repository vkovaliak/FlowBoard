using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.DTOs.Archive;
using FlowBoard.Infrastructure.Configurations;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace FlowBoard.Infrastructure.Tracking;

public class CosmosArchiveJobTracker : IArchiveJobTracker
{
    private readonly Container _container;

    public CosmosArchiveJobTracker(
        CosmosClient cosmosClient, IOptions<CosmosOptions> options)
    {
        var config = options.Value;
        _container = cosmosClient.GetContainer(
            config.DatabaseName, config.ContainerName);
    }

    public async Task TrackStartedAsync(Guid boardId)
    {
        var record = new ArchiveJobRecord
        {
            BoardId = boardId,
            Status = "Processing",
            StartedAt = DateTime.UtcNow
        };

        await _container.CreateItemAsync(
            record, new PartitionKey(boardId.ToString()));
    }

    public async Task TrackCompletedAsync(Guid boardId, string blobUrl)
    {
        var record = new ArchiveJobRecord
        {
            BoardId = boardId,
            Status = "Completed",
            StartedAt = DateTime.UtcNow,
            CompletedAt = DateTime.UtcNow,
            BlobUrl = blobUrl
        };

        await _container.UpsertItemAsync(
            record, new PartitionKey(boardId.ToString()));
    }

    public async Task TrackFailedAsync(Guid boardId, string errorMessage)
    {
        var record = new ArchiveJobRecord
        {
            BoardId = boardId,
            Status = "Failed",
            StartedAt = DateTime.UtcNow,
            CompletedAt = DateTime.UtcNow,
            ErrorMessage = errorMessage
        };

        await _container.UpsertItemAsync(
            record, new PartitionKey(boardId.ToString()));
    }
}