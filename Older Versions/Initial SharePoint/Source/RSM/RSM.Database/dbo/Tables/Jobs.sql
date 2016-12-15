CREATE TABLE [dbo].[Jobs] (
    [JobCode]            NVARCHAR (50)  NOT NULL,
    [JobDescription]     NVARCHAR (MAX) NOT NULL,
    [DateAdded]          DATE           NOT NULL,
    [DisplayDescription] NVARCHAR (MAX) NULL,
    [Credentials]        NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_Jobs] PRIMARY KEY CLUSTERED ([JobCode] ASC)
);

