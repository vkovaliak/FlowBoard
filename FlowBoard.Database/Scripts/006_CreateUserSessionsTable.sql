CREATE TABLE [UserSessions] 
(
    [Id]             UNIQUEIDENTIFIER       PRIMARY KEY,
    [UserId]         UNIQUEIDENTIFIER       NOT NULL,
    [Token]          NVARCHAR(500)          NOT NULL,
    [ExpiryTime]     DATETIME2              NOT NULL,
    FOREIGN KEY ([UserId]) REFERENCES Users([Id]) ON DELETE CASCADE
);