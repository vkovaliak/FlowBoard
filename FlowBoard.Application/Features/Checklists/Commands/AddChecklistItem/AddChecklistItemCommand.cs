using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Checklists.Commands.AddChecklistItem;

public record AddChecklistItemCommand(
    Guid BoardId,
    Guid CardId,
    string Text)
    : IRequest<Result<Guid>>;