CREATE TABLE [CardAssignees] (
    [CardId]            UNIQUEIDENTIFIER            NOT NULL,
    [UserId]            UNIQUEIDENTIFIER            NOT NULL,
    CONSTRAINT [PK_CardAssignees] PRIMARY KEY ([CardId], [UserId]),
    CONSTRAINT [FK_CardAssignees_Cards] FOREIGN KEY ([CardId]) REFERENCES Cards ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_CardAssignees_Users] FOREIGN KEY ([UserId]) REFERENCES Users ([Id]) ON DELETE CASCADE
);