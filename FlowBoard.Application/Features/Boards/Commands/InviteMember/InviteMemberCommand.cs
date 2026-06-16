using FlowBoard.Domain.Enums;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Boards.Commands.InviteMember;

public record InviteMemberCommand(
    Guid BoardId, 
    string Email,
    BoardRole Role) 
    : IRequest<Result<bool>>;