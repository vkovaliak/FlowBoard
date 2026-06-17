namespace FlowBoard.Application.Abstractions;

public interface ICardLabelRepository
{
    Task AttachAsync(Guid cardId, Guid labelId);
    Task DetachAsync(Guid cardId, Guid labelId);
}