using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace FlowBoard.Infrastructure.Jobs;

public class ArchiveJob : IArchiveJob
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly IArchiveMessagePublisher _publisher;
    private readonly ILogger<ArchiveJob> _logger;

    public ArchiveJob(
        IUnitOfWorkFactory uowFactory,
        IArchiveMessagePublisher publisher,
        ILogger<ArchiveJob> logger)
    {
        _uowFactory = uowFactory;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task ProcessArchivedBoardsAsync()
    {
        using var uow = _uowFactory.Create();

        var boards = await uow.BoardRepository.GetByArchiveStatusAsync(
            ArchiveStatus.Pending);

        var boardList = boards.ToList();

        _logger.LogInformation(
            "ArchiveJob: found {Count} archived boards to process",
            boardList.Count);

        foreach (var board in boardList)
        {
            try
            {
                await _publisher.PublishArchiveMessageAsync(board.Id);

                await uow.BoardRepository.UpdateArchiveStatusAsync(
                    board.Id, ArchiveStatus.Queued);

                _logger.LogInformation(
                    "ArchiveJob: board {BoardId} queued for archiving",
                    board.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "ArchiveJob: failed to queue board {BoardId}",
                    board.Id);
            }
        }
        uow.Commit();
    }
}