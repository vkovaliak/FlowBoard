using MediatR;

namespace FlowBoard.Application.Features.Users.Commands.CreateUser;

public record CreateUserCommand(string Email, string Password) : IRequest<Guid>;