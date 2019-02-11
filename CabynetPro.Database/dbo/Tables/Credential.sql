CREATE TABLE [dbo].[Credential] (
    [Id]           INT           IDENTITY (1, 1) NOT NULL,
    [Username]     VARCHAR (100) NOT NULL,
    [PasswordData] VARCHAR (100) NOT NULL,
    [PasswordSalt] VARCHAR (100) NOT NULL,
    [Surname]      VARCHAR (100) NULL,
    [OtherNames]   VARCHAR (100) NULL,
    [PhoneNumber]  VARCHAR (100) NULL,
    [UserRoles]    INT           DEFAULT ((1)) NOT NULL,
    [UserState]    INT           DEFAULT ((1)) NOT NULL,
    [DateCreated]  DATETIME      DEFAULT (getdate()) NOT NULL,
    [IsDeleted]    BIT           DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

