namespace FlowBoard.Application.Abstractions;

public interface IArchiveMessagePublisher
{
    Task PublishArchiveMessageAsync(Guid boardId);
}