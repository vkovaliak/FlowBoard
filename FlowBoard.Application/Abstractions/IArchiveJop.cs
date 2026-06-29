namespace FlowBoard.Application.Abstractions;

public interface IArchiveJob
{
    Task ProcessArchivedBoardsAsync();
}