ALTER TABLE [Cards] 
ADD [DueDate]       DATETIME2 NULL,
    [IsCompleted]   BIT NOT NULL DEFAULT 0;