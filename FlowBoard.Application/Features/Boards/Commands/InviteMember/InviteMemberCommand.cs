using MediatR;

namespace FlowBoard.Application.Features.Boards.Commands.InviteMember;

public record InviteMemberCommand(
    Guid BoardId, 
    string Email, 
    Guid CurrentUserId) : IRequest<bool>;