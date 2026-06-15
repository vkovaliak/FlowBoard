CREATE TABLE [CardAttachments] 
(
    [Id]            UNIQUEIDENTIFIER        PRIMARY KEY,
    [CardId]        UNIQUEIDENTIFIER        NOT NULL,
    [FileName]      NVARCHAR(255)           NOT NULL,
    [BlobUrl]       NVARCHAR(2048)          NOT NULL,
    [ContentType]   NVARCHAR(100)           NOT NULL,
    [UploadedAt]    DATETIME2               NOT NULL DEFAULT GETUTCDATE(),
    [UploadetBy]    UNIQUEIDENTIFIER        NOT NULL,
    CONSTRAINT FK_CardAttachments_Cards FOREIGN KEY (CardId) REFERENCES Cards(Id) ON DELETE CASCADE
);