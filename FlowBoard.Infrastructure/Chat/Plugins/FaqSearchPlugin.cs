using System.ComponentModel;
using FlowBoard.Application.Abstractions;
using Microsoft.SemanticKernel;

namespace FlowBoard.Infrastructure.Chat.Plugins;

public sealed class FaqSearchPlugin
{
    private readonly ISearchContextProvider _contextProvider;

    public FaqSearchPlugin(ISearchContextProvider contextProvider)
    {
        _contextProvider = contextProvider;
    }

    [KernelFunction("search_faq")]
    [Description(
        "Searches the FlowBoard knowledge base (FAQ) for information about the product, " +
        "such as boards, cards, lists, archiving, accounts and security. ")]
    public async Task<string> SearchFaqAsync(
        [Description("The user's question to look up in the FAQ knowledge base.")]
        string query)
    {
        return await _contextProvider.GetContextAsync(query);
    }
}