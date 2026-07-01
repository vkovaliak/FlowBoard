using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.DTOs.AIChat;
using FlowBoard.Infrastructure.Chat.Prompts;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace FlowBoard.Infrastructure.Chat;

public sealed class SemanticKernelChatService : IChatService
{
    private readonly IChatCompletionService _chatCompletion;
    private readonly ISearchContextProvider _contextProvider;

    public SemanticKernelChatService(
        Kernel kernel,
        ISearchContextProvider contextProvider)
    {
        _chatCompletion = kernel.GetRequiredService<IChatCompletionService>();
        _contextProvider = contextProvider;
    }

    public async Task<ChatResponse> SendMessageAsync(string request)
    {
        var context = await _contextProvider.GetContextAsync(request);

        var chatHistory = new ChatHistory();
        chatHistory.AddSystemMessage(ChatPrompts.BuildSystemPrompt(context));
        chatHistory.AddUserMessage(request);

        var response = await _chatCompletion.GetChatMessageContentAsync(
            chatHistory);

        return new ChatResponse(
            Answer: response.Content ?? string.Empty);
    }
}