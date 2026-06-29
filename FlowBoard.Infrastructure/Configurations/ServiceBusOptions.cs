namespace FlowBoard.Infrastructure.Configurations;

public class ServiceBusOptions
{
    public const string SectionName = "ServiceBus";

    public string ConnectionString { get; set; } = default!;
    public string ArchiveQueueName { get; set; } = default!;
}