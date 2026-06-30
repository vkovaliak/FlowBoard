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

    public ArchiveBoardProcessor(
        IUnitOfWorkFactory uowFactory,
        IFileStorageService fileStorage,
        ILogger<ArchiveBoardProcessor> logger)
    {
        _uowFactory = uowFactory;
        _fileStorage = fileStorage;
        _logger = logger;
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

            var board = await uow.BoardRepository.GetForArchiveAsync(boardId);

            if (board is null)
            {
                _logger.LogWarning(
                    "ArchiveBoardFunction: board {BoardId} not found", boardId);

                await SetStatusAsync(boardId, ArchiveStatus.Failed);

                return;
            }

            var json = JsonSerializer.Serialize(board, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            var fileName = $"{boardId}.json";

            var blobUrl = await _fileStorage.UploadAsync(
                stream, fileName, StorageConstants.ArchivedBoardsContainer);

            _logger.LogInformation(
                "ArchiveBoardFunction: board {BoardId} saved to {Url}",
                boardId, blobUrl);

            await SetStatusAsync(boardId, ArchiveStatus.Completed);

            _logger.LogInformation(
                "ArchiveBoardFunction: board {BoardId} completed", boardId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "ArchiveBoardFunction: failed for board {BoardId}", boardId);

            await SetStatusAsync(boardId, ArchiveStatus.Failed);
            throw;
        }
    }
}