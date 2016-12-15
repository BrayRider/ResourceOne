CREATE TABLE [dbo].[AccessHistory]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Accessed] DATETIME NOT NULL, 
    [PersonId] INT NOT NULL, 
    [PortalId] INT NOT NULL, 
    [ReaderId] INT NOT NULL, 
    [Type] INT NOT NULL, 
    [Reason] INT NULL, 
    CONSTRAINT [FK_AccessHistory_People] FOREIGN KEY ([PersonId]) REFERENCES [dbo].[People]([PersonID]), 
    CONSTRAINT [FK_AccessHistory_Portal] FOREIGN KEY ([PortalId]) REFERENCES [dbo].[Portal]([Id]), 
    CONSTRAINT [FK_AccessHistory_Reader] FOREIGN KEY ([ReaderId]) REFERENCES [dbo].[Reader]([Id])
)
