CREATE TABLE [dbo].[PSImportStatus] (
    [id]         INT            IDENTITY (1, 1) NOT NULL,
    [importDate] DATETIME       NOT NULL,
    [filename]   NVARCHAR (MAX) NOT NULL,
    [success]    BIT            NOT NULL,
    [message]    NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_PSImportStatus] PRIMARY KEY CLUSTERED ([id] ASC)
);

