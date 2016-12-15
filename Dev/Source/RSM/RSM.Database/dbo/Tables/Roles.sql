CREATE TABLE [dbo].[Roles] (
    [RoleID]   INT            IDENTITY (1, 1) NOT NULL,
    [RoleName] NVARCHAR (50)  NOT NULL,
    [RoleDesc] NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED ([RoleID] ASC)
);

