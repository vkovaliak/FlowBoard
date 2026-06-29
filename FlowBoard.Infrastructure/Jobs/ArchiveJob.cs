using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace FlowBoard.Infrastructure.Jobs;

public class ArchiveJob : IArchiveJob
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly ILogger<ArchiveJob> _logger;

    public ArchiveJob(
        IUnitOfWorkFactory uowFactory,
        ILogger<ArchiveJob> logger)
    {
        _uowFactory = uowFactory;
        _logger = logger;
    }

    public async Task ProcessArchivedBoardsAsync()
    {
        using var uow = _uowFactory.Create();

        var boards = await uow.BoardRepository.GetByArchiveStatusAsync(
            ArchiveStatus.Archived);

        var boardList = boards.ToList();

        _logger.LogInformation(
            "ArchiveJob: found {Count} archived boards to process",
            boardList.Count);

        foreach (var board in boardList)
        {
            _logger.LogInformation(
                "ArchiveJob: would process board {BoardId} ({Name})",
                board.Id, board.Name);
        }
    }
}