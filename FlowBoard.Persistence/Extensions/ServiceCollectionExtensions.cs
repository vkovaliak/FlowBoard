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
        services.AddScoped<ICardRepository, CardRepository>();
        services.AddScoped<ICardAttachmentRepository, CardAttachmentsRepository>();
        services.AddScoped<ICommentAttachmentRepository, CommentAtachmentsRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<IListRepository, ListRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserSessionRepository, UserSessionRepository>();

        return services;
    }
}