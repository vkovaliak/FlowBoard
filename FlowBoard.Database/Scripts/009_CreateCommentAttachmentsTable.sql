CREATE TABLE [CommentAttachments] 
(
    [Id]            UNIQUEIDENTIFIER        PRIMARY KEY,
    [CommentId]     UNIQUEIDENTIFIER        NOT NULL,
    [FileName]      NVARCHAR(255)           NOT NULL,
    [BlobUrl]       NVARCHAR(2048)          NOT NULL,
    [ContentType]   NVARCHAR(100)           NOT NULL,
    [UploadedAt]    DATETIME2               NOT NULL DEFAULT GETUTCDATE(),
    [UploadetBy]    UNIQUEIDENTIFIER        NOT NULL,
    CONSTRAINT FK_CommentAttachments_Comments FOREIGN KEY (CommentId) REFERENCES Comments(Id) ON DELETE CASCADE
);