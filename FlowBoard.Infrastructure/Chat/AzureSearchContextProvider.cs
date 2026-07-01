using System.Text;
using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using FlowBoard.Application.Abstractions;
using FlowBoard.Infrastructure.Configurations;
using Microsoft.Extensions.Options;
using FlowBoard.Domain.Constants;


namespace FlowBoard.Infrastructure.Chat;

public sealed class AzureSearchContextProvider : ISearchContextProvider
{
    private readonly SearchClient _searchClient;

    public AzureSearchContextProvider(IOptions<AzureAiOptions> options)
    {
        var opts = options.Value;

        _searchClient = new SearchClient(
            new Uri(opts.SearchEndpoint),
            opts.SearchIndexName,
            new AzureKeyCredential(opts.SearchApiKey));
    }

    public async Task<string> GetContextAsync(string query)
    {
        var searchOptions = new SearchOptions
        {
            Size = ChatConstants.MaxSearchResults,
            Select = { 
                ChatConstants.ContentField, 
                ChatConstants.TitleField 
            },
            QueryType = SearchQueryType.Semantic
        };

        var response = await _searchClient.SearchAsync<SearchDocument>(
            query, searchOptions);

        var contextBuilder = new StringBuilder();

        await foreach (var result in response.Value.GetResultsAsync())
        {
            if (result.Document.TryGetValue(ChatConstants.ContentField, 
                out var contentObj)
                && contentObj is string content)
            {
                result.Document.TryGetValue(ChatConstants.TitleField, 
                    out var titleObj);

                var source = titleObj as string 
                    ?? ChatConstants.DefaultSourceName;

                contextBuilder
                    .AppendLine($"[Source: {source}]: {content}")
                    .AppendLine(ChatConstants.SourceSeparator);
            }
        }

        return contextBuilder.Length > 0
            ? contextBuilder.ToString()
            : ChatConstants.NoContextFound;
    }
}