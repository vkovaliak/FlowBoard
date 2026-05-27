using FlowBoard.Application.Abstractions;

namespace FlowBoard.Persistence.Repositories;

public class RepositoryFactory : IRepositoryFactory
{
    public IUserRepository CreateUserRepository(IUnitOfWork unitOfWork)
    {
        return new UserRepository(unitOfWork.Connection, unitOfWork.Transaction);
    }
}