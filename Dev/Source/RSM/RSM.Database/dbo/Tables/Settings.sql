CREATE TABLE [dbo].[Settings] (
    [Id]        INT            NOT NULL,
    [Viewable]  BIT            CONSTRAINT [DF_Settings_Viewable] DEFAULT ((0)) NOT NULL,
    [SystemId]  INT            NOT NULL,
    [InputType] NVARCHAR (10)  CONSTRAINT [DF_Settings_InputType] DEFAULT (N'text') NOT NULL,
    [Name]      NVARCHAR (50)  NOT NULL,
    [Label]     NVARCHAR (250) NOT NULL,
    [Value]     NVARCHAR (250) NOT NULL,
    [OrderBy]   INT            CONSTRAINT [DF_Settings_OrderBy] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Settings] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Settings_ExternalSystems] FOREIGN KEY ([SystemId]) REFERENCES [dbo].[ExternalSystems] ([Id])
);



