CREATE TABLE BoardMembers (
    [UserId]            UNIQUEIDENTIFIER            NOT NULL,
    [BoardId]           UNIQUEIDENTIFIER            NOT NULL,
    CONSTRAINT [PK_BoardMembers] PRIMARY KEY ([UserId], [BoardId]),
    CONSTRAINT [FK_BoardMembers_Users] FOREIGN KEY ([UserId]) REFERENCES Users ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_BoardMembers_Boards] FOREIGN KEY ([BoardId]) REFERENCES Boards ([Id]) ON DELETE CASCADE
);