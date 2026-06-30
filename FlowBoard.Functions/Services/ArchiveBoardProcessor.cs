using System.Text;
using System.Text.Json;
using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.Constants;
using FlowBoard.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace FlowBoard_Functions.Services;

public class ArchiveBoardProcessor : IArchiveBoardProcessor
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly IFileStorageService _fileStorage;
    private readonly ILogger<ArchiveBoardProcessor> _logger;
    private readonly IArchiveJobTracker _tracker;

    public ArchiveBoardProcessor(
        IUnitOfWorkFactory uowFactory,
        IFileStorageService fileStorage,
        ILogger<ArchiveBoardProcessor> logger,
        IArchiveJobTracker tracker)
    {
        _uowFactory = uowFactory;
        _fileStorage = fileStorage;
        _logger = logger;
        _tracker = tracker;
    }

    private async Task SetStatusAsync(Guid boardId, ArchiveStatus status)
    {
        using var uow = _uowFactory.Create();
        await uow.BoardRepository.UpdateArchiveStatusAsync(
            boardId, status);

        uow.Commit();
    }

    public async Task ProcessAsync(Guid boardId)
    {
       using var uow = _uowFactory.Create();
        try
        {
            await uow.BoardRepository.UpdateArchiveStatusAsync(
                boardId, ArchiveStatus.Processing);
                
            uow.Commit();
            await _tracker.TrackStartedAsync(boardId);

            var board = await uow.BoardRepository.GetForArchiveAsync(boardId);
            if (board is null)
            {
                await SetStatusAsync(boardId, ArchiveStatus.Failed);
                await _tracker.TrackFailedAsync(boardId, "Board not found");
                return;
            }

            var json = JsonSerializer.Serialize(board, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            var blobUrl = await _fileStorage.UploadAsync(
                stream, $"{boardId}.json", StorageConstants.ArchivedBoardsContainer);

            await SetStatusAsync(boardId, ArchiveStatus.Completed);
            await _tracker.TrackCompletedAsync(boardId, blobUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "ArchiveBoardFunction: failed for board {BoardId}", boardId);
            await SetStatusAsync(boardId, ArchiveStatus.Failed);
            await _tracker.TrackFailedAsync(boardId, ex.Message);
            throw;
        }
    }
}