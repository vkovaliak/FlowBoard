using FlowBoard.Domain.Entities;

namespace FlowBoard.Application.Abstractions;

public interface IUserSessionRepository : IBaseRepository<UserSession, Guid>
{
    Task<UserSession?> GetByTokenAsync(string token);
}