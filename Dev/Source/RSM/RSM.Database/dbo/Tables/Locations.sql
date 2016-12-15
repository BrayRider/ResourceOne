CREATE TABLE [dbo].[Locations] (
    [LocationID]   INT           IDENTITY (1, 1) NOT NULL,
    [LocationName] NVARCHAR (100) NOT NULL,
    [DateAdded]    DATE          NOT NULL,
    CONSTRAINT [PK_Locations] PRIMARY KEY CLUSTERED ([LocationID] ASC)
);

