CREATE TABLE [dbo].[JCLRoleRule] (
    [ID]       INT           IDENTITY (1, 1) NOT NULL,
    [JobCode]  NVARCHAR (50) NOT NULL,
    [DeptID]   NVARCHAR (50) NOT NULL,
    [Location] INT           NOT NULL,
    CONSTRAINT [PK_JCLToRole] PRIMARY KEY CLUSTERED ([ID] ASC)
);

