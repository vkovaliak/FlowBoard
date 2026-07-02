using FlowBoard.Domain.DTOs.AIChat;
using FluentResults;
using MediatR;

namespace FlowBoard.Application.Features.Chat.Commands.SendChatMessage;

public sealed record SendChatMessageCommand(
    string Message)
    : IRequest<Result<ChatResponse>>;