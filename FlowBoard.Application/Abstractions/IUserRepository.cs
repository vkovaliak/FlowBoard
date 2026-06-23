using FlowBoard.Domain.Entities;

namespace FlowBoard.Application.Abstractions;

public interface IUserRepository : IBaseRepository<User, Guid>
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByExternalIdAsync(string provider, string externalId);
}