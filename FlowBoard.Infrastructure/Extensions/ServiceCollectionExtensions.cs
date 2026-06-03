using FlowBoard.Application.Abstractions;
using FlowBoard.Infrastructure.Auth;
using Microsoft.Extensions.DependencyInjection;

namespace FlowBoard.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IJwtProvider, JwtProvider>();

        return services;
    }
}