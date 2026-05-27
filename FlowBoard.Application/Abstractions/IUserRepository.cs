using FlowBoard.Domain.Entities;

namespace FlowBoard.Application.Abstractions;

public interface IUserRepository
{
    Task CreateAsync(User user);
}