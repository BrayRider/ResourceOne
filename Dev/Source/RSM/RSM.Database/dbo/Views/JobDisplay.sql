

CREATE VIEW [dbo].[JobDisplay]
AS
SELECT      JobCode, JobDescription + ' (' + CAST(JobCode AS varchar) + ')' as JobDescription, DateAdded 
FROM         dbo.jobs

