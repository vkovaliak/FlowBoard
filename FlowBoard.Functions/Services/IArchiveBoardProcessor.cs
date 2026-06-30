namespace FlowBoard_Functions.Services;

public interface IArchiveBoardProcessor
{
    Task ProcessAsync(Guid boardId);
}