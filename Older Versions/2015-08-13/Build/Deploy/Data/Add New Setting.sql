USE [R1SM_Track]
GO

INSERT INTO [dbo].[Settings]
           ([Id]
           ,[Viewable]
           ,[SystemId]
           ,[InputType]
           ,[Name]
           ,[Label]
           ,[Value]
           ,[OrderBy])
     VALUES
           (25
           ,1
           ,2
           ,'text'
           ,'S2Import.Contractors'
           ,'Comma delimited list of contractors to get from S2'
           ,'S & B,Mustang'
           ,5)
GO

