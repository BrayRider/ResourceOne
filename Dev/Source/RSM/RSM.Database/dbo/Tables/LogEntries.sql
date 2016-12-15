CREATE TABLE [dbo].[LogEntries] (
    [ID]        INT            IDENTITY (1, 1) NOT NULL,
    [EventDate] DATETIME       NOT NULL,
    [Source]    INT            NOT NULL,
    [Severity]  INT            NOT NULL,
    [Message]   NVARCHAR (MAX) NOT NULL,
    [Details]   TEXT           NULL,
    CONSTRAINT [PK_LogEntries] PRIMARY KEY CLUSTERED ([ID] ASC)
);

