using FlowBoard.Application.Abstractions;
using FlowBoard.Persistence.Connection;
using FlowBoard.Persistence.Repositories;
using FlowBoard.Persistence.UnitOfWork;
using Microsoft.Extensions.DependencyInjection;

namespace FlowBoard.Persistence.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services)
    {
        services.AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>();
        services.AddSingleton<IUnitOfWorkFactory, UnitOfWorkFactory>();

        services.AddScoped<IBoardRepository, BoardRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}