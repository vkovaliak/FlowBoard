using System.Reflection;
using FlowBoard.Application.Features.Users.Commands.CreateUser;
using Microsoft.Extensions.DependencyInjection;

namespace FlowBoard.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });

        return services;
    }
}