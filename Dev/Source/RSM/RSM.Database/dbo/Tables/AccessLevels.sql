CREATE TABLE [dbo].[AccessLevels] (
    [AccessLevelID]      INT            NOT NULL,
    [AccessLevelName]    NVARCHAR (50)  NOT NULL,
    [AccessLevelDesc]    NVARCHAR (MAX) NULL,
    [ReaderGroupID]      INT            NULL,
    [TimeSpecID]         INT            NULL,
    [ThreatLevelGroupID] INT            NULL,
    [Missing]            BIT            CONSTRAINT [DF_AccessLevels_Missing] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_AccessLevels] PRIMARY KEY CLUSTERED ([AccessLevelID] ASC)
);

