namespace FlowBoard.Infrastructure.Configurations;

public sealed class AzureAiOptions
{
    public const string SectionName = "AzureAiOptions";

    public string OpenAiEndpoint { get; set; } = string.Empty;
    public string OpenAiApiKey { get; set; } = string.Empty;
    public string DeploymentName { get; set; } = string.Empty;
    public string SearchEndpoint { get; set; } = string.Empty;
    public string SearchApiKey { get; set; } = string.Empty;
    public string SearchIndexName { get; set; } = string.Empty;
}