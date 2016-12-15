CREATE TABLE [dbo].[BatchHistory] (
    [Id]       INT            NOT NULL IDENTITY,
    [SystemId] INT            NOT NULL,
    [RunStart] DATETIME       NOT NULL,
    [RunEnd]   DATETIME       NOT NULL,
    [Filename] NVARCHAR (250) NULL,
    [Outcome]  INT            NOT NULL,
    [Message]  NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_BatchHistory] PRIMARY KEY CLUSTERED ([Id] ASC)
);

