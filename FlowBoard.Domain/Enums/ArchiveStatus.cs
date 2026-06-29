namespace FlowBoard.Domain.Enums;

public enum ArchiveStatus
{
    None = 0,
    Archived = 1,
    Queued = 2,
    Processing = 3,
    Completed = 4,
    Failed = 5
}