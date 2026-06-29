ALTER TABLE [Boards]
ADD [ArchiveStatus]     INT         NOT NULL DEFAULT 0,
    [ArchivedAt]        DATETIME2   NULL;