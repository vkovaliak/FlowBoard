using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Boards.Commands.RemoveMember;

public record RemoveMemberCommand(
    Guid BoardId,
    Guid UserId)
    : IRequest<Result<bool>>;