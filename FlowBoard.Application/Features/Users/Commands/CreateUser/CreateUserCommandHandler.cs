using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.Entities;
using MediatR;

namespace FlowBoard.Application.Features.Users.Commands.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Guid>
{
    private readonly IUserRepository _userRepository;

    public CreateUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        var user = new User
        {
            Id = Guid.NewGuid(),
            EmailAddress = request.Email,
            PasswordHash = passwordHash,
        };

        await _userRepository.CreateAsync(user);

        return user.Id;
    }
}