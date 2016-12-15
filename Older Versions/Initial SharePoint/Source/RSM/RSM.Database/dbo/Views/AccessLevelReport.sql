


CREATE VIEW [dbo].[AccessLevelReport]
AS
select AccessLevelName, AccessLevelDesc, ReaderGroupID, dbo.FlatPeopleWithLevel(AccessLevelID) as people
from AccessLevels 



