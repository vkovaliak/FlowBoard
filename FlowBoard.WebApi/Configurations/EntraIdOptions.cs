namespace FlowBoard.WebApi.Configurations;

public class EntraIdOptions
{
    public const string SectionName = "EntraId";

    public string Authority { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string CallbackPath { get; set; } = "/signin-microsoft";
}