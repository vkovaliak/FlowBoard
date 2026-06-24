namespace FlowBoard.Infrastructure.Configurations;

public class EntraIdOptions
{
    public const string SectionName = "EntraId";

    public string Authority { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
}