namespace FlowBoard.Infrastructure.Configurations;

public class CosmosOptions
{
    public const string SectionName = "Cosmos";

    public string ConnectionString { get; set; } = default!;
    public string DatabaseName { get; set; } = default!;
    public string ContainerName { get; set; } = default!;
}