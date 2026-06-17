CREATE TABLE [Labels] 
(
    [Id]            UNIQUEIDENTIFIER      NOT NULL,
    [BoardId]       UNIQUEIDENTIFIER      NOT NULL,
    [Name]          NVARCHAR(50)          NOT NULL,
    [Color]         NVARCHAR(7)           NOT NULL,
    [CreatedAt]     DATETIME2             NOT NULL DEFAULT GETUTCDATE(),
    [CreatedBy]     UNIQUEIDENTIFIER      NOT NULL,
    [UpdatedAt]     DATETIME2             NULL,
    [UpdatedBy]     UNIQUEIDENTIFIER      NULL,
    CONSTRAINT [PK_Labels] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Labels_Boards] FOREIGN KEY ([BoardId]) REFERENCES Boards ([Id]) ON DELETE CASCADE
);