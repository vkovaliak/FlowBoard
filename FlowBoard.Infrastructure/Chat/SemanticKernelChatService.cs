using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.DTOs.AIChat;
using FlowBoard.Infrastructure.Chat.Prompts;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;

namespace FlowBoard.Infrastructure.Chat;

public sealed class SemanticKernelChatService : IChatService
{
    private readonly Kernel _kernel;

    public SemanticKernelChatService(Kernel kernel)
    {
        _kernel = kernel;
    }

    public async Task<ChatResponse> SendMessageAsync(string request)
    {
        var settings = new AzureOpenAIPromptExecutionSettings
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        };

        var chatCompletion = _kernel.GetRequiredService<IChatCompletionService>();

        var history = new ChatHistory();

        history.AddSystemMessage(ChatPrompts.AssistantPrompt);
        history.AddUserMessage(request);

        var response = await chatCompletion.GetChatMessageContentAsync(
            history,
            settings,
            _kernel);

        return new ChatResponse(Answer: response.ToString());
    }
}