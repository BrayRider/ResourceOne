CREATE TABLE [dbo].[PeopleRoles] (
    [ID]          INT IDENTITY (1, 1) NOT NULL,
    [PersonID]    INT NOT NULL,
    [RoleID]      INT NOT NULL,
    [IsException] BIT CONSTRAINT [DF_PeopleRoles_IsException] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_PeopleRoles] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [PeopleRoleRole] FOREIGN KEY ([RoleID]) REFERENCES [dbo].[Roles] ([RoleID]) ON DELETE CASCADE
);

