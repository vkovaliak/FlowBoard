namespace FlowBoard.Infrastructure.Configurations;

public class FrontendOptions
{
    public const string SectionName = "FrontendOptions";

    public string BaseUrl { get; set; } = string.Empty;
    public string ExternalCallbackPath { get; set; } = "/external-callback";
    public string LoginPath { get; set; } = "/login";
}