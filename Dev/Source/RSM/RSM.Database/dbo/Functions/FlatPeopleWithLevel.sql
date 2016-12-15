


	CREATE FUNCTION [dbo].[FlatPeopleWithLevel] 
(
	@levelID int
)
RETURNS varchar(2000)
AS
BEGIN
	
	DECLARE @names varchar(2000)
select @names = COALESCE(@names + ', ', '') + FirstName + ' ' + LastName
from People  
where PersonId  in 
	(select PersonID from PeopleRoles where RoleID in 
		(select RoleID from AccessLevelRoles where AccessLevelID  =  @levelID))

order by LastName, FirstName
	
	RETURN (@names)

END


