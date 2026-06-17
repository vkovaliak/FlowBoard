using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Cards.Commands.UnassignMember;

public record UnassignMemberCommand(
    Guid BoardId,
    Guid CardId,
    Guid UserId)
    : IRequest<Result<bool>>;