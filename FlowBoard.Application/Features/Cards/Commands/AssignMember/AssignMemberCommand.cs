using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Cards.Commands.AssignMember;

public record AssignMemberCommand(
    Guid BoardId,
    Guid CardId,
    Guid UserId)
    : IRequest<Result<bool>>;