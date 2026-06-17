using FlowBoard.Domain.DTOs.CardAssignee;

namespace FlowBoard.Application.Abstractions;

public interface ICardAssigneeRepository
{
    Task AssignAsync(Guid cardId, Guid userId);
    Task UnassignAsync(Guid cardId, Guid userId);
}