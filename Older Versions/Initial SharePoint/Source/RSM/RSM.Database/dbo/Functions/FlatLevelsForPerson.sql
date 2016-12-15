



	CREATE FUNCTION [dbo].[FlatLevelsForPerson] 
(
	@PersonID int
)
RETURNS varchar(2000)
AS
BEGIN
	
	DECLARE @names varchar(2000)
	
	select  @names = COALESCE(@names + ', ', '') + AccessLevelName  
		from AccessLevels  
		where AccessLevelID  in (select AccessLevelID from AccessLevelRoles where AccessLevelRoles.RoleID in (select RoleID from PeopleRoles where PeopleRoles.PersonID = @PersonID))
		
		order by AccessLevelName 
	
	RETURN (@names)

END



