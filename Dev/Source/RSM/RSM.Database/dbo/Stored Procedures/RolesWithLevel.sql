





CREATE procedure [dbo].[RolesWithLevel]
	@LevelID int
as
-- Procedure Name : RolesAvialableForPerson
-- Created By : Donavan Stanley
-- Created On : 2/8/2011 8:16:53 AM
begin
	-- local variables

	-- set nocount on and default isolation level
	set nocount on
	set transaction isolation level Read Committed
	
		select * 
		from Roles  
		where RoleID  in (select RoleID from AccessLevelRoles where AccessLevelID  = @LevelID)
		 

end





