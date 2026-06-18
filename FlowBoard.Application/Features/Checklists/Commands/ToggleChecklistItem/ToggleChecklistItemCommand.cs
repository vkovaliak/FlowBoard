using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Checklists.Commands.ToggleChecklistItem;

public record ToggleChecklistItemCommand(
    Guid BoardId,
    Guid CardId,
    Guid ItemId)
    : IRequest<Result<bool>>;