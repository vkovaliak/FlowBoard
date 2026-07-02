using FlowBoard.Application.Abstractions;
using FlowBoard.Domain.DTOs.AIChat;
using FlowBoard.Infrastructure.Chat.Prompts;
using Microsoft.SemanticKernel;
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

        var response = await _kernel.InvokePromptAsync(
            ChatPrompts.AssistantPrompt,
            new KernelArguments(settings)
            {
                ["request"] = request
            });

        return new ChatResponse(Answer: response.ToString());
    }
}