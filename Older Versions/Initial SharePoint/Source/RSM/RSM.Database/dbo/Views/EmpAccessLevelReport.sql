



CREATE VIEW [dbo].[EmpAccessLevelReport]
AS
select firstname, lastname, Deptdescr, jobdescr, dbo.flatlevelsforperson(personid) as levels
from people





