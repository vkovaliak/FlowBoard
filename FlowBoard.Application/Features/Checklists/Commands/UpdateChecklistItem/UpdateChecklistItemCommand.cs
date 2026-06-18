using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Checklists.Commands.UpdateChecklistItem;

public record UpdateChecklistItemCommand(
    Guid BoardId,
    Guid CardId,
    Guid ItemId,
    string Text)
    : IRequest<Result<bool>>;