ALTER TABLE [Users]
ALTER COLUMN [PasswordHash] NVARCHAR(500)   NULL;

ALTER TABLE [Users]
ADD [ExternalProvider]   NVARCHAR(50)     NULL,
    [ExternalId]         NVARCHAR(255)    NULL;