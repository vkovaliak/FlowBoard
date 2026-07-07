namespace FlowBoard_Functions.Services;

public interface IRestoreBoardProcessor
{
    Task ProcessAsync(Guid boardId);
}