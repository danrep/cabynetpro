CREATE TABLE [dbo].[FileAuditLog] (
    [Id]                BIGINT   IDENTITY (1, 1) NOT NULL,
    [FileInformationId] BIGINT   NOT NULL,
    [AuditPeriod]       DATETIME NOT NULL,
    [AuditType]         INT      NOT NULL,
    [AuditCommitUserId] INT      NOT NULL,
    [IsDeleted]         BIT      NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

