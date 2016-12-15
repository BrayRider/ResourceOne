CREATE TABLE [dbo].[Departments] (
    [DeptID]    NVARCHAR (50)  NOT NULL,
    [DeptDescr] NVARCHAR (MAX) NOT NULL,
    [DateAdded] DATETIME       NOT NULL,
    CONSTRAINT [PK_Departments] PRIMARY KEY CLUSTERED ([DeptID] ASC)
);

