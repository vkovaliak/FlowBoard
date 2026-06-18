CREATE TABLE [ChecklistItems]
(
    [Id]            UNIQUEIDENTIFIER    NOT NULL,
    [CardId]        UNIQUEIDENTIFIER    NOT NULL,
    [Text]          NVARCHAR(500)       NOT NULL,
    [IsCompleted]   BIT                 NOT NULL DEFAULT 0,
    [Position]      INT                 NOT NULL,
    [CreatedAt]     DATETIME2           NOT NULL DEFAULT GETUTCDATE(),
    [CreatedBy]     UNIQUEIDENTIFIER    NOT NULL,
    [UpdatedAt]     DATETIME2           NULL,
    [UpdatedBy]     UNIQUEIDENTIFIER    NULL,
    CONSTRAINT [PK_ChecklistItems] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ChecklistItems_Cards] FOREIGN KEY ([CardId]) REFERENCES Cards ([Id]) ON DELETE CASCADE
);