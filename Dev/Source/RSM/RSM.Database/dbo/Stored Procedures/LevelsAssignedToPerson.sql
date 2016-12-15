




CREATE procedure [dbo].[LevelsAssignedToPerson]
	@PersonID int
as
-- Procedure Name : RolesAvialableForPerson
-- Created By : Donavan Stanley
-- Created On : 2/8/2011 8:16:53 AM
begin
	-- local variables

	-- set nocount on and default isolation level
	set nocount on
	set transaction isolation level Read Committed
	
		select distinct AccessLevelID, AccessLevelName, ReaderGroupID, TimeSpecID, ThreatLevelGroupID 
		from AccessLevels  
		where AccessLevelID  in (select AccessLevelID from AccessLevelRoles where AccessLevelRoles.RoleID in (select RoleID from PeopleRoles where PeopleRoles.PersonID = @PersonID))
		order by AccessLevelID 

end




