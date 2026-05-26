using FlowBoard.Application.Abstractions;
using FlowBoard.Persistence.Connection;
using FlowBoard.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace FlowBoard.Persistence.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services)
    {
        services.AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>();
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}