

CREATE VIEW [dbo].[JCLRuleDisplay]
AS
select ru.ID,ru.JobCode, j.JobDescription, d.DeptDescr, l.LocationName  
 
from JCLRoleRule ru
inner join Jobs j on j.JobCode = ru.JobCode 
inner join Departments d on d.DeptID =  ru.DeptID
inner join Locations l on l.LocationID = ru.Location 

