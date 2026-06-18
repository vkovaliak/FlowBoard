using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Checklists.Commands.DeleteChecklistItem;

public record DeleteChecklistItemCommand(
    Guid BoardId,
    Guid CardId,
    Guid ItemId)
    : IRequest<Result<bool>>;