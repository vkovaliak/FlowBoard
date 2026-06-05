namespace FlowBoard.WebApi.Configurations;

public class CorsOptions
{
    public const string SectionName = "Cors";

    public required string PolicyName { get; set; }

    public required string AllowedOrigin { get; set; }
}