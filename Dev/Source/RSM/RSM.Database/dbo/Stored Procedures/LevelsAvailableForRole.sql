


CREATE procedure [dbo].[LevelsAvailableForRole]
	@RoleID int
as
-- Procedure Name : LevelsAvailableForRole
-- Created By : Donavan Stanley
-- Created On : 2/8/2011 8:16:53 AM
begin
	-- local variables

	-- set nocount on and default isolation level
	set nocount on
	set transaction isolation level Read Committed
	
		select AccessLevelID, AccessLevelName, AccessLevelDesc
		from AccessLevels 
		where AccessLevels.AccessLevelID not in (select AccessLevelID from AccessLevelRoles where AccessLevelRoles.RoleID = @RoleID)
		order by AccessLevels.AccessLevelName 

end


