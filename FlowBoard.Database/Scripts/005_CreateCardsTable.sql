CREATE TABLE [Cards]
(
    [Id]              UNIQUEIDENTIFIER      NOT NULL PRIMARY KEY,
    [ListId]          UNIQUEIDENTIFIER      NOT NULL,
    [Name]            NVARCHAR(100)         NOT NULL,
    [Description]     NVARCHAR(500)         NULL,
    [Position]        INT                   NOT NULL,
    [CreatedAt]       DATETIME2             NOT NULL DEFAULT GETUTCDATE(),
    [CreatedBy]       UNIQUEIDENTIFIER      NOT NULL,
    [UpdatedAt]       DATETIME2             NULL,
    [UpdatedBy]       UNIQUEIDENTIFIER      NULL,
    CONSTRAINT [FK_Cards_Lists] FOREIGN KEY ([ListId]) REFERENCES [Lists] ([Id]) ON DELETE CASCADE
);