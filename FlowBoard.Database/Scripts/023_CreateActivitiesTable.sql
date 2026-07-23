CREATE TABLE [Activities] (
    [Id]              UNIQUEIDENTIFIER    PRIMARY KEY DEFAULT NEWID(),
    [BoardId]         UNIQUEIDENTIFIER    NOT NULL,
    [UserId]          UNIQUEIDENTIFIER    NOT NULL,
    [ActionType]      INT                 NOT NULL,
    [EntityType]      INT                 NOT NULL,
    [EntityId]        UNIQUEIDENTIFIER    NULL,
    [Description]     NVARCHAR(500)       NOT NULL,
    [CreatedAt]       DATETIME2           NOT NULL DEFAULT SYSUTCDATETIME(),

    CONSTRAINT FK_Activities_Boards FOREIGN KEY ([BoardId]) REFERENCES Boards(Id) ON DELETE CASCADE,
    CONSTRAINT FK_Activities_Users FOREIGN KEY ([UserId]) REFERENCES Users(Id)
);