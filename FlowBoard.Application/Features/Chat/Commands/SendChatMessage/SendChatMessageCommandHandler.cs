using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.DTOs.AIChat;
using FlowBoard.Domain.Enums;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Chat.Commands.SendChatMessage;

public sealed class SendChatMessageQueryHandler
    : IRequestHandler<SendChatMessageCommand, Result<ChatResponse>>
{
    private readonly IChatService _chatService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWorkFactory _uowFactory;

    public SendChatMessageQueryHandler(
        IChatService chatService,
        ICurrentUserService currentUserService,
        IUnitOfWorkFactory uowFactory)
    {
        _chatService = chatService;
        _currentUserService = currentUserService;
        _uowFactory = uowFactory;
    }

    public async Task<Result<ChatResponse>> Handle(
        SendChatMessageCommand request,
        CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.GetId();

        using var uow = _uowFactory.Create();

        var user = await uow.UserRepository.GetByIdAsync(currentUserId);
        if (user is null)
        {
            return Result.Fail("User is not found");
        }

        if (user.SubscriptionPlan != SubscriptionPlan.Pro)
        {
            return Result.Fail("AI Chat is a Pro feature. Upgrade to unlock it.");
        }

        return await _chatService.SendMessageAsync(request.Message);
    }
}