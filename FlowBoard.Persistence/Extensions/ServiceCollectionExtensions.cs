using FlowBoard.Application.Abstractions;
using FlowBoard.Persistence.Configurations;
using FlowBoard.Persistence.Connection;
using FlowBoard.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;
using FlowBoard.Database;
using Microsoft.Extensions.Options;

namespace FlowBoard.Persistence.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services)
    {
        services.AddOptions<DatabaseOptions>().BindConfiguration(DatabaseOptions.SectionName);

        services.AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddSingleton(provider =>
        {
            var options = provider.GetRequiredService<IOptions<DatabaseOptions>>().Value;
            
            return new DatabaseInitializer(options.ConnectionString);
        });

        return services;
    }
}