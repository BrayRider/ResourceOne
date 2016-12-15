CREATE TABLE [dbo].[Portal]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
	[Name] VARCHAR(200) NOT NULL, 
	[Added] DATETIME NOT NULL DEFAULT SYSDATETIME(), 
	[LocationId] INT NOT NULL, 
	[NetworkAddress] VARCHAR(255) NULL, 
	[DeviceType] INT NULL, 
	[Capabilities] VARCHAR(100) NULL, 
	CONSTRAINT [FK_Portal_Location] FOREIGN KEY ([LocationId]) REFERENCES [dbo].[Locations]([LocationID])
)
