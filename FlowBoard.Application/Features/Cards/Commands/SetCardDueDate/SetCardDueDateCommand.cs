using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Cards.Commands.SetCardDueDate;

public record SetCardDueDateCommand(
    Guid BoardId,
    Guid CardId,
    DateTime? DueDate)
    : IRequest<Result<bool>>;