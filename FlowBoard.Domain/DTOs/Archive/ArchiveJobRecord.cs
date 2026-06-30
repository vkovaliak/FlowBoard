using Newtonsoft.Json;

namespace FlowBoard.Domain.DTOs.Archive;

public class ArchiveJobRecord
{
    [JsonProperty("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    [JsonProperty("boardId")]
    public Guid BoardId { get; set; }
    public string Status { get; set; } = default!;
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? BlobUrl { get; set; }
    public string? ErrorMessage { get; set; }
}