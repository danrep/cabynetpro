CREATE TABLE [dbo].[FileInformation] (
    [Id]                  BIGINT         IDENTITY (1, 1) NOT NULL,
    [DateCreated]         DATETIME       NOT NULL,
    [UserCreated]         INT            NOT NULL,
    [FileNameSource]      VARCHAR (100)  NOT NULL,
    [FileNameSystem]      VARCHAR (100)  NOT NULL,
    [FileNameDescription] VARCHAR (1000) NOT NULL,
    [FileTag]             VARCHAR (MAX)  NOT NULL,
    [FileType]            VARCHAR (20)   NOT NULL,
    [FileSize]            BIGINT         DEFAULT ((0)) NOT NULL,
    [CategoryId]          INT            NOT NULL,
    [FolderId]            BIGINT         NOT NULL,
    [IsDeleted]           BIT            NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

