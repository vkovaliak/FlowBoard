using Azure.Storage.Blobs;
using FlowBoard.Application.Abstractions;
using FlowBoard.Infrastructure.Auth;
using FlowBoard.Infrastructure.Configurations;
using FlowBoard.Infrastructure.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FlowBoard.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IExternalAuthService, ExternalAuthService>();
        services.AddScoped<IJwtProvider, JwtProvider>();

        services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<IOptions<AzureBlobOptions>>().Value;
                        
            return new BlobServiceClient(options.ConnectionString);
        });

        services.AddScoped<IFileStorageService, AzureBlobStorageService>();

        return services;
    }
}