CREATE TABLE [Users]
(
    [Id]              UNIQUEIDENTIFIER      NOT NULL PRIMARY KEY,
    [EmailAddress]    NVARCHAR(255)         NOT NULL UNIQUE,
    [PasswordHash]    NVARCHAR(500)         NOT NULL,
    [SignupDate]      DATETIME2             NOT NULL DEFAULT GETUTCDATE()
);