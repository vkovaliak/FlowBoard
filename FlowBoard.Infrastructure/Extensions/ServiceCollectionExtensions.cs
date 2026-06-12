using FlowBoard.Application.Abstractions;
using FlowBoard.Infrastructure.Auth;
using FlowBoard.Infrastructure.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace FlowBoard.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IJwtProvider, JwtProvider>();

        services.AddScoped<IFileStorageService, AzureBlobStorageService>();

        return services;
    }
}