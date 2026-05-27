namespace FlowBoard.Application.Abstractions;

public interface IRepositoryFactory
{
    IUserRepository CreateUserRepository(IUnitOfWork unitOfWork);
}