using FlowBoard.Domain.Enums;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Boards.Commands.ChangeMemberRole;

public record ChangeMemberRoleCommand(
    Guid BoardId,
    Guid UserId,
    BoardRole NewRole) 
    : IRequest<Result<bool>>;