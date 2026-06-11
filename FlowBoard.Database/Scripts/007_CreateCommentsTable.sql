CREATE TABLE [Comments]
(
    [Id]            UNIQUEIDENTIFIER      NOT NULL PRIMARY KEY,
    [CardId]        UNIQUEIDENTIFIER      NOT NULL,
    [Message]       NVARCHAR(MAX)         NOT NULL,
    [CreatedAt]     DATETIME2             NOT NULL DEFAULT GETUTCDATE(),
    [CreatedBy]     UNIQUEIDENTIFIER      NOT NULL,
    [UpdatedAt]     DATETIME2             NULL,
    [UpdatedBy]     UNIQUEIDENTIFIER      NULL,
    CONSTRAINT [FK_Comments_Cards] FOREIGN KEY ([CardId]) REFERENCES [Cards] ([Id]) ON DELETE CASCADE
);