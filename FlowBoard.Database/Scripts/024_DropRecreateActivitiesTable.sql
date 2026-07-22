IF OBJECT_ID('Activities', 'U') IS NOT NULL
    DROP TABLE Activities;

CREATE TABLE [Activities] (
    [Id]            UNIQUEIDENTIFIER    NOT NULL PRIMARY KEY DEFAULT NEWID(),
    [CardId]        UNIQUEIDENTIFIER    NOT NULL,
    [BoardId]       UNIQUEIDENTIFIER    NOT NULL,
    [UserId]        UNIQUEIDENTIFIER    NOT NULL,
    [ActionType]    INT                 NOT NULL,
    [Description]   NVARCHAR(500)       NOT NULL,
    [CreatedAt]     DATETIME2           NOT NULL DEFAULT GETUTCDATE(),

    CONSTRAINT FK_Activities_Cards FOREIGN KEY ([CardId]) REFERENCES Cards(Id) ON DELETE CASCADE,
    CONSTRAINT FK_Activities_Users FOREIGN KEY ([UserId]) REFERENCES Users(Id)
);