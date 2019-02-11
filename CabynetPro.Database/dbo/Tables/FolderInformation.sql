CREATE TABLE [dbo].[FolderInformation] (
    [Id]                BIGINT         IDENTITY (1, 1) NOT NULL,
    [DateCreated]       DATETIME       NOT NULL,
    [UserCreated]       INT            NOT NULL,
    [FolderName]        VARCHAR (100)  NOT NULL,
    [CategoryId]        INT            NOT NULL,
    [FolderDescription] VARCHAR (1000) NOT NULL,
    [ParentFolderId]    BIGINT         NOT NULL,
    [IsDeleted]         BIT            NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

