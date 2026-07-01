using FlowBoard.Domain.DTOs.AIChat;

namespace FlowBoard.Application.Abstractions;

public interface IChatService
{
    Task<ChatResponse> SendMessageAsync(string request);
}