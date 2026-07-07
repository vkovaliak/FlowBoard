using System.Text.Json;
using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.Constants;
using FlowBoard.Domain.DTOs.Archive;
using FlowBoard.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace FlowBoard_Functions.Services;

public class RestoreBoardProcessor : IRestoreBoardProcessor
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly IFileStorageService _fileStorage;
    private readonly ILogger<RestoreBoardProcessor> _logger;

    public RestoreBoardProcessor(
        IUnitOfWorkFactory uowFactory,
        IFileStorageService fileStorage,
        ILogger<RestoreBoardProcessor> logger)
    {
        _uowFactory = uowFactory;
        _fileStorage = fileStorage;
        _logger = logger;
    }

    public async Task ProcessAsync(Guid boardId)
    {
        try
        {
            var json = await _fileStorage.DownloadAsync(
                boardId, StorageConstants.ArchivedBoardsContainer);

            if (json is null)
            {
                _logger.LogError(
                    "RestoreBoard: blob not found for board {BoardId}", 
                    boardId);

                await SetStatusAsync(boardId, ArchiveStatus.Failed);
                return;
            }

            var board = JsonSerializer.Deserialize<BoardArchiveDto>(json);

            if (board is null)
            {
                _logger.LogError(
                    "RestoreBoard: failed to deserialize board {BoardId}", 
                    boardId);

                await SetStatusAsync(boardId, ArchiveStatus.Failed);
                return;
            }

            using (var uow = _uowFactory.Create())
            {
                await uow.BoardRepository.RestoreBoardContentAsync(board);

                await uow.BoardRepository.UpdateArchiveStatusAsync(
                    boardId, ArchiveStatus.None);

                uow.Commit();
            }

            var blobUrl = await FindBlobUrlAsync(boardId);
            if (blobUrl is not null)
            {
                await _fileStorage.DeleteAsync(blobUrl);
            }

            _logger.LogInformation(
                "RestoreBoard: board {BoardId} restored successfully", 
                boardId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "RestoreBoard: failed for board {BoardId}", boardId);

            await SetStatusAsync(boardId, ArchiveStatus.Failed);
            throw;
        }
    }

    private async Task<string?> FindBlobUrlAsync(Guid boardId)
    {
        var urls = await _fileStorage.ListBlobUrlsAsync(
            StorageConstants.ArchivedBoardsContainer);

        return urls.FirstOrDefault(
            u => u.EndsWith($"{boardId}.json"));
    }

    private async Task SetStatusAsync(
        Guid boardId, ArchiveStatus status)
    {
        using var uow = _uowFactory.Create();
        await uow.BoardRepository.UpdateArchiveStatusAsync(
            boardId, status);
        uow.Commit();
    }
}