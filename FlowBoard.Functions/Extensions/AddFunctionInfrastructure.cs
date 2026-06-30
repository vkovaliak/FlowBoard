using FlowBoard.Application.Abstractions;
using FlowBoard.Infrastructure.Storage;
using FlowBoard.Infrastructure.Messaging;
using Microsoft.Extensions.DependencyInjection;
using FlowBoard_Functions.Services;
using FlowBoard.Persistence.Connection;
using FlowBoard.Persistence.UnitOfWork;
using Microsoft.Extensions.Options;
using FlowBoard.Infrastructure.Configurations;
using Azure.Storage.Blobs;

namespace FlowBoard.Infrastructure.Functions;

public static class DependencyInjection
{
    public static IServiceCollection AddFunctionInfrastructure(
        this IServiceCollection services)
    {
        services.AddScoped<IFileStorageService, AzureBlobStorageService>();

        services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<IOptions<AzureBlobOptions>>().Value;
                        
            return new BlobServiceClient(options.ConnectionString);
        });

        services.AddSingleton<IUnitOfWorkFactory, UnitOfWorkFactory>();
        services.AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>();

        services.AddScoped<IArchiveBoardProcessor, ArchiveBoardProcessor>();

        return services;
    }
}