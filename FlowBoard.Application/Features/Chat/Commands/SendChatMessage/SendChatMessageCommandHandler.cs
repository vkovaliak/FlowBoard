using System.Net;
using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.DTOs.AIChat;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Chat.Commands.SendChatMessage;

public sealed class SendChatMessageQueryHandler
    : IRequestHandler<SendChatMessageCommand, Result<ChatResponse>>
{
    private readonly IChatService _chatService;

    public SendChatMessageQueryHandler(IChatService chatService)
    {
        _chatService = chatService;
    }

    public async Task<Result<ChatResponse>> Handle(
        SendChatMessageCommand request,
        CancellationToken cancellationToken)
    {
        return await _chatService.SendMessageAsync(request.Message);
    }
}