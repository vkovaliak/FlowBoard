using MediatR;
using FlowBoard.Domain.Enums;

namespace FlowBoard.Application.Features.Activities.Events;

public record BoardActivityEvent(
    Guid BoardId,
    Guid UserId,
    ActivityAction ActionType,
    ActivityEntityType EntityType,
    Guid? EntityId,
    string Description
) : INotification;