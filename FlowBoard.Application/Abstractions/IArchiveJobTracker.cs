namespace FlowBoard.Application.Abstractions;

public interface IArchiveJobTracker
{
    Task TrackStartedAsync(Guid boardId);
    Task TrackCompletedAsync(Guid boardId, string blobUrl);
    Task TrackFailedAsync(Guid boardId, string errorMessage);
}