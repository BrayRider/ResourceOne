CREATE TABLE [dbo].[ExternalApplicationKeys] (
    [EntityType] NVARCHAR (50) NOT NULL,
    [InternalId] INT           NOT NULL,
    [ExternalId] NVARCHAR (100) NOT NULL,
    [SystemId]   INT           NOT NULL,
    [Added] DATETIME NOT NULL DEFAULT SYSDATETIME(), 
    [ExternalEntityLastUpdated] DATETIME NULL, 
    CONSTRAINT [PK_ExternalApplicationKeys] PRIMARY KEY CLUSTERED ([EntityType] ASC, [InternalId] ASC, [ExternalId] ASC, [SystemId] ASC),
    CONSTRAINT [FK_ExternalApplicationKeys_ExternalSystems] FOREIGN KEY ([SystemId]) REFERENCES [dbo].[ExternalSystems] ([Id])
);

