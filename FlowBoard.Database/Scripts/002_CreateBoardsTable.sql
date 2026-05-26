CREATE TABLE [Boards]
(
    [Id]              UNIQUEIDENTIFIER      NOT NULL PRIMARY KEY,
    [Name]            NVARCHAR(100)         NOT NULL,
    [IsPublic]        BIT                   NOT NULL DEFAULT 0,
    [CreatedAt]       DATETIME2             NOT NULL DEFAULT GETUTCDATE(),
    [CreatedBy]       UNIQUEIDENTIFIER      NOT NULL,
    [UpdatedAt]       DATETIME2             NULL,
    [UpdatedBy]       UNIQUEIDENTIFIER      NULL
);