CREATE TABLE [Lists]
(
    [Id]              UNIQUEIDENTIFIER      NOT NULL PRIMARY KEY,
    [BoardId]         UNIQUEIDENTIFIER      NOT NULL,
    [Name]            NVARCHAR(100)         NOT NULL,
    [Position]        INT                   NOT NULL,
    [CreatedAt]       DATETIME2             NOT NULL DEFAULT GETUTCDATE(),
    [CreatedBy]       UNIQUEIDENTIFIER      NOT NULL,
    [UpdatedAt]       DATETIME2             NULL,
    [UpdatedBy]       UNIQUEIDENTIFIER      NULL,
    CONSTRAINT [FK_Lists_Boards] FOREIGN KEY ([BoardId]) REFERENCES [Boards] ([Id]) ON DELETE CASCADE
);