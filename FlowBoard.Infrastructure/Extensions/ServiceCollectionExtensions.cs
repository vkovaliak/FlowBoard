using Azure.Storage.Blobs;
using FlowBoard.Application.Abstractions;
using FlowBoard.Infrastructure.Auth;
using FlowBoard.Infrastructure.Chat;
using FlowBoard.Infrastructure.Configurations;
using FlowBoard.Infrastructure.Jobs;
using FlowBoard.Infrastructure.Messaging;
using FlowBoard.Infrastructure.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;

namespace FlowBoard.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IArchiveJob, ArchiveJob>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IChatService, SemanticKernelChatService>();
        services.AddScoped<IExternalAuthService, ExternalAuthService>();
        services.AddScoped<IFileStorageService, AzureBlobStorageService>();
        services.AddScoped<IJwtProvider, JwtProvider>();
        services.AddScoped<ISearchContextProvider, AzureSearchContextProvider>();

        services.AddSingleton(sp =>
        {
            var opts = sp.GetRequiredService<IOptions<AzureAiOptions>>().Value;

            var builder = Kernel.CreateBuilder();
            builder.AddAzureOpenAIChatCompletion(
                deploymentName: opts.DeploymentName,
                endpoint: opts.OpenAiEndpoint,
                apiKey: opts.OpenAiApiKey);

            return builder.Build();
        });

        services.AddSingleton<IArchiveMessagePublisher, ServiceBusMessagePublisher>();

        services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<IOptions<AzureBlobOptions>>().Value;
                        
            return new BlobServiceClient(options.ConnectionString);
        });

        return services;
    }
}