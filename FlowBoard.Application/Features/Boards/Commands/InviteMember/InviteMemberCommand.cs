using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Boards.Commands.InviteMember;

public record InviteMemberCommand(
    Guid BoardId, 
    string Email) 
    : IRequest<Result<bool>>;