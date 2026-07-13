namespace FlowBoard.Domain.Constants;

public static class HangfireConstants
{
    public const string DashboardPath = "/hangfire";

    public const string ArchiveBoardsJobId = "archive-boards-job";

    public const string ArchiveBoardsJobCron = "*/1 * * * *";
}