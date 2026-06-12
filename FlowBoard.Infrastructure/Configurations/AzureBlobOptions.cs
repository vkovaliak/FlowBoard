namespace FlowBoard.Infrastructure.Configurations;
public class AzureBlobOptions
{
    public const string SectionName = "AzureBlobOptions";

    public string ConnectionString { get; set; } = string.Empty;
    public string AttachmentsContainer { get; set; } = string.Empty;
}