DECLARE @DateCleaner varchar(15)
SET @DateCleaner = '2015-01-01'

DELETE FROM [R1SM_Track].[dbo].[LogEntries]
WHERE [EventDate] < @DateCleaner


DELETE FROM [R1SM_Track].[dbo].[AccessHistory]
WHERE [Accessed] < @DateCleaner

