CREATE TABLE [dbo].[AccessLevelRoles] (
    [ID]            INT IDENTITY (1, 1) NOT NULL,
    [AccessLevelID] INT NOT NULL,
    [RoleID]        INT NOT NULL,
    CONSTRAINT [PK_AccessLevelRoles] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [AccessLevelRoleLevel] FOREIGN KEY ([AccessLevelID]) REFERENCES [dbo].[AccessLevels] ([AccessLevelID]),
    CONSTRAINT [LevelToRole] FOREIGN KEY ([RoleID]) REFERENCES [dbo].[Roles] ([RoleID]) ON DELETE CASCADE
);

