CREATE TABLE [dbo].[ExternalSystems] (
    [Id]        INT           NOT NULL,
    [Name]      NVARCHAR (50) NOT NULL,
    [Direction] INT           CONSTRAINT [DF_ExternalSystems_Direction] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_ExternalSystems] PRIMARY KEY CLUSTERED ([Id] ASC)
);



