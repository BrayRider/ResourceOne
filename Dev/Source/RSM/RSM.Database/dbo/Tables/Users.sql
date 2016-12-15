CREATE TABLE [dbo].[Users] (
    [PersonID] INT           NOT NULL,
    [Password] NCHAR (256)   NOT NULL,
    [Admin]    BIT           NOT NULL,
    [Username] NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([PersonID] ASC)
);

